using IceScheduler;
using IceScheduler.Formatters;
using IceScheduler.Parsers;
using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScheduleTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Arguments: '{0}'", string.Join("','", args));

            if (args.Length < 1)
            {
                PrintUsage();
                return;
            }

            List<IceSlot> slots = null;

            string inputType = null;
            string inputPath = null;
            string outputType = null;
            string outputPath = null;
            string processType = null;
            string processArgs = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-r")
                {
                    inputType = args[++i];
                }
                else if (args[i] == "-w")
                {
                    outputType = args[++i];
                }
                else if (args[i] == "-i")
                {
                    inputPath = args[++i];
                }
                else if (args[i] == "-o")
                {
                    outputPath = args[++i];
                }
                else if (args[i] == "-p")
                {
                    processType = args[++i];
                }
                else if (args[i] == "-a")
                {
                    processArgs = args[++i];
                }
                else
                {
                    Console.WriteLine("Unrecognized switch: {0}", args[i]);
                    PrintUsage();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(inputType))
            {
                if (string.IsNullOrEmpty(inputPath))
                {
                    Console.WriteLine("Input path cannot be empty if specifying an input type.");
                    PrintUsage();
                    return;
                }

                if (inputType.ToLower() == "ravens")
                {
                    RavensScheduleParser parser = new RavensScheduleParser();
                    slots = parser.Parse(inputPath);
                }
                else if (inputType.ToLower() == "pcaha")
                {
                    // TODO: How to get game type in here?
                    PcahaScheduleParser parser = new PcahaScheduleParser();
                    slots = parser.Parse(GameType.League, inputPath);
                }
                else if (inputType.ToLower() == "flat")
                {
                    FlatListParser parser = new FlatListParser();
                    slots = parser.Parse(inputPath);
                }
                else
                {
                    Console.WriteLine("Unrecognized input type: {0}", inputType);
                    PrintUsage();
                    return;
                }
            }

            Console.WriteLine("Read {0} slots.", slots.Count);

            if (!string.IsNullOrEmpty(processType))
            {
                if (processType.ToLower() == "sort")
                {
                    Console.WriteLine("Sorting slots using '{0}'.", processArgs);
                    if (processArgs.ToLower() == "start")
                    {
                        slots = slots.OrderBy(s => s.IceTime.Start).ToList();
                    }
                    else
                    {
                        Console.WriteLine("Unrecognized sort type: {0}", processArgs);
                        PrintUsage();
                        return;
                    }
                }
                else if (processType.ToLower() == "rebase")
                {
                    Console.WriteLine("Rebasing slots using '{0}'.", processArgs);

                    DateTime oldStartDate = slots.First().IceTime.Start.Date;
                    DateTime newStartDate = DateTime.Parse(processArgs);
                    TimeSpan delta = newStartDate - oldStartDate;

                    slots = slots.Select(s =>
                    {
                        s.IceTime.Adjust(delta);
                        return s;
                    }).ToList();
                }
                else if (processType.ToLower() == "filter")
                {
                    Console.WriteLine("Filtering slots using '{0}'.", processArgs);
                    if (processArgs.ToLower() == "richmond")
                    {
                        // Richmond slots
                        slots = slots.Where(s => s.ToString().Contains("Richmond")).ToList();
                    }
                    else if (processArgs.ToLower() == "homegame")
                    {
                        // Richmond home games
                        slots = slots.Where(s => s is GameSlot).ToList();
                        slots = slots.Where(s => (s as GameSlot).HomeTeam.Association == Association.RichmondGirls).ToList();
                    }
                    else if (processArgs.ToLower() == "conflict")
                    {
                        slots = slots.Where(s => s is GameSlot).ToList();
                        slots = slots.Where(s => (s as GameSlot).IceTime.Start == DateTime.MinValue).ToList();
                    }
                    else if (processArgs.ToLower() == "nonconflict")
                    {
                        slots = slots.Where(s => s is GameSlot).ToList();
                        slots = slots.Where(s => (s as GameSlot).IceTime.Start != DateTime.MinValue).ToList();
                    }
                    else if (processArgs.ToLower() == "nongame")
                    {
                        slots = slots.Where(s => !(s is GameSlot)).ToList();
                    }
                    else 
                    {
                        Regex regex = new Regex(@"week(\d+)-(\d+)");
                        Match match = regex.Match(processArgs);
                        if (match.Success)
                        {
                            string weekNumber = match.Groups[1].Value;
                            string yearNumber = match.Groups[2].Value;
                            DateTime startOfWeek = FirstDateOfWeekISO8601(Convert.ToInt32(yearNumber), Convert.ToInt32(weekNumber));
                            DateTime endOfWeek = startOfWeek + TimeSpan.FromDays(6);

                            slots = slots.Where(s => (s.IceTime.Start.Date >= startOfWeek && s.IceTime.Start.Date <= endOfWeek)).ToList();
                        }
                        else
                        {
                            Console.WriteLine("Unrecognized filter type: {0}", processArgs);
                            PrintUsage();
                            return;
                        }
                    }
                }
                else if (processType == "print")
                {
                    slots.ForEach(s => Console.WriteLine(s.ToString()));
                }
                else if (processType == "check")
                {
                    MultipleIceSlotFinder finder = new MultipleIceSlotFinder();
                    List<IceSlot> multipleSlots = finder.FindMultiples(slots);

                    Console.WriteLine("Found {0} slots on the same day as other slots.", multipleSlots.Count);
                    multipleSlots.ForEach(s => Console.WriteLine(s.ToString()));
                }
                else
                {
                    Console.WriteLine("Unrecognized process type: {0}", processType);
                    PrintUsage();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(outputType))
            {
                if (string.IsNullOrEmpty(outputPath))
                {
                    Console.WriteLine("Output path cannot be empty if specifying an output type.");
                    PrintUsage();
                    return;
                }

                Console.WriteLine("Writing {0} slots.", slots.Count);

                if (outputType.ToLower() == "flat")
                {
                    FlatListFormatter formatter = new FlatListFormatter();
                    formatter.WriteSchedule(slots, outputPath);
                }
                else if (outputType.ToLower() == "ravens")
                {
                    RavensScheduleFormatter formatter = new RavensScheduleFormatter();
                    formatter.WriteSchedule(slots, outputPath);
                }
                else if (outputType.ToLower() == "import")
                {
                    TeamPagesFormatter formatter = new TeamPagesFormatter();
                    formatter.WriteSchedule(slots, outputPath);
                }
                else
                {
                    Console.WriteLine("Unrecognized output type: {0}", outputType);
                    PrintUsage();
                    return;
                }
            }

            WaitForKey();
        }

        // http://stackoverflow.com/questions/662379/calculate-date-from-week-number
        static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }
        static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    Don't be a dumbass!");

            WaitForKey();
        }

        static void WaitForKey()
        {
            if (Debugger.IsAttached)
            {
                Console.Write("Hit any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
