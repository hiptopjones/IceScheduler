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
            List<string> inputPaths = new List<string>();
            string outputType = null;
            string outputPath = null;
            List<string> processTypes = new List<string>();
            List<string> processArguments = new List<string>();

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
                    inputPaths.Add(args[++i]);
                }
                else if (args[i] == "-o")
                {
                    outputPath = args[++i];
                }
                else if (args[i] == "-p")
                {
                    processTypes.Add(args[++i]);
                    processArguments.Add(string.Empty);
                }
                else if (args[i] == "-a")
                {
                    if (processTypes.Count == 0)
                    {
                        Console.WriteLine("Mismatched -p / -a switches.");
                        PrintUsage();
                        return;
                    }

                    processArguments[processTypes.Count - 1] = args[++i];
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
                if (!inputPaths.Any())
                {
                    Console.WriteLine("Input path cannot be empty if specifying an input type.");
                    PrintUsage();
                    return;
                }

                if (inputType.ToLower() == "ravens")
                {
                    RavensScheduleParser parser = new RavensScheduleParser();
                    slots = inputPaths.SelectMany(inputPath => parser.Parse(inputPath)).ToList();
                }
                else if (inputType.ToLower() == "pcaha")
                {
                    // TODO: How to get game type in here?
                    PcahaScheduleParser parser = new PcahaScheduleParser();
                    slots = inputPaths.SelectMany(inputPath => parser.Parse(GameType.Playoff, inputPath)).ToList();
                }
                else if (inputType.ToLower() == "flat")
                {
                    FlatListParser parser = new FlatListParser();
                    slots = inputPaths.SelectMany(inputPath => parser.Parse(inputPath)).ToList();
                }
                else if (inputType.ToLower() == "teamlink")
                {
                    TeamLinkParser parser = new TeamLinkParser();
                    slots = inputPaths.SelectMany(teamGrouping => parser.ParseSchedule(teamGrouping)).ToList();
                }
                else
                {
                    Console.WriteLine("Unrecognized input type: {0}", inputType);
                    PrintUsage();
                    return;
                }
            }

            Console.WriteLine("Read {0} slots.", slots.Count);

            if (processTypes.Any())
            {
                for (int i = 0; i < processTypes.Count; i++)
                {
                    string processType = processTypes[i];
                    string processTypeLower = processType.ToLower();

                    string processArgument = processArguments[i];
                    string processArgumentLower = processArgument.ToLower();

                    if (processTypeLower == "sort")
                    {
                        Console.WriteLine("Sorting slots using '{0}'.", processArgument);
                        if (processArgumentLower == "start")
                        {
                            slots = slots.OrderBy(s => s.IceTime.Start).ToList();
                        }
                        else
                        {
                            Console.WriteLine("Unrecognized sort type: {0}", processArgument);
                            PrintUsage();
                            return;
                        }
                    }
                    else if (processTypeLower == "rebase")
                    {
                        Console.WriteLine("Rebasing slots using '{0}'.", processArgument);

                        DateTime oldStartDate = slots.First().IceTime.Start.Date;
                        DateTime newStartDate = DateTime.Parse(processArgument);
                        TimeSpan delta = newStartDate - oldStartDate;

                        slots = slots.Select(s =>
                        {
                            s.IceTime.Adjust(delta);
                            return s;
                        }).ToList();
                    }
                    else if (processTypeLower == "filter")
                    {
                        Console.WriteLine("Filtering slots using '{0}'.", processArgument);

                        if (processArgumentLower == "richmond")
                        {
                            // Richmond slots
                            slots = slots.Where(s => s.ToString().Contains("Richmond")).ToList();
                        }
                        else if (processArgumentLower == "homegame")
                        {
                            // Richmond home games
                            slots = slots.Where(s => s is GameSlot).ToList();
                            slots = slots.Where(s => (s as GameSlot).HomeTeam.Association == Association.RichmondGirls).ToList();
                        }
                        else if (processArgumentLower == "conflict")
                        {
                            slots = slots.Where(s => s is GameSlot).ToList();
                            slots = slots.Where(s => (s as GameSlot).IceTime.Start == DateTime.MinValue).ToList();
                        }
                        else if (processArgumentLower == "nonconflict")
                        {
                            slots = slots.Where(s => s is GameSlot).ToList();
                            slots = slots.Where(s => (s as GameSlot).IceTime.Start != DateTime.MinValue).ToList();
                        }
                        else if (processArgumentLower == "nongame")
                        {
                            slots = slots.Where(s => !(s is GameSlot)).ToList();
                        }
                        else if (processArgumentLower == "available")
                        {
                            slots = slots.Where(s => s is AvailableSlot).ToList();
                        }
                        else if (processArgumentLower.StartsWith("teams"))
                        {
                            List<IceSlot> filteredSlots = new List<IceSlot>();

                            string[] teamNames = processArgument.Split(new[] { ' ' });
                            foreach (string teamName in teamNames.Skip(1))
                            {
                                List<Team> teams = ParsingUtilities.ParseRavensTeams(teamName);
                                foreach (Team team in teams)
                                {
                                    filteredSlots.AddRange(slots.Where(s =>
                                    {
                                        TeamBasedIceSlot teamSlot = s as TeamBasedIceSlot;
                                        return teamSlot != null && teamSlot.HasParticipatingTeam(team);
                                    }));
                                }
                            }

                            slots = filteredSlots;
                        }
                        else
                        {
                            Regex regex = new Regex(@"week(\d+)-(\d+)");
                            Match match = regex.Match(processArgument);
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
                                Console.WriteLine("Unrecognized filter type: {0}", processArgument);
                                PrintUsage();
                                return;
                            }
                        }
                    }
                    else if (processTypeLower == "print")
                    {
                        slots.ForEach(s => Console.WriteLine(s.ToString()));
                    }
                    else if (processTypeLower == "check")
                    {
                        MultipleIceSlotFinder finder = new MultipleIceSlotFinder();
                        List<IceSlot> multipleSlots = finder.FindMultiples(slots);

                        Console.WriteLine("Found {0} slots on the same day as other slots.", multipleSlots.Count);
                        multipleSlots.ForEach(s => Console.WriteLine(s.ToString()));
                    }
                    else if (processTypeLower == "breakdown")
                    {
                        IceSlotReporter reporter = new IceSlotReporter();
                        reporter.ReportTeamSlotBreakdown(slots);
                    }
                    else
                    {
                        Console.WriteLine("Unrecognized process type: {0}", processTypeLower);
                        PrintUsage();
                        return;
                    }
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

                if (inputPaths.Any(path => (path.ToLower() == outputPath.ToLower())))
                {
                    Console.WriteLine("Input and output paths must be distinct.");
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
                    formatter.WriteAllSchedules(slots, outputPath);
                }
                else if (outputType.ToLower() == "matrix")
                {
                    ConflictMatrixFormatter formatter = new ConflictMatrixFormatter();
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
