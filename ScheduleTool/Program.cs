﻿using IceScheduler;
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
                    slots = inputPaths.SelectMany(inputPath => parser.Parse(GameType.League, inputPath)).ToList();
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
                else if (inputType.ToLower() == "invoice")
                {
                    InvoiceParser parser = new InvoiceParser();
                    slots = inputPaths.SelectMany(inputPath => parser.Parse(inputPath)).ToList();
                }
                else if (inputType.ToLower() == "sportngin")
                {
                    SportNginParser parser = new SportNginParser();
                    slots = inputPaths.SelectMany(inputPath => parser.Parse(inputPath)).ToList();
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
                    else if (processTypeLower == "collapse")
                    {
                        Console.WriteLine("Collapsing duplicate slots.");

                        Dictionary<string, IceSlot> slotMap = new Dictionary<string, IceSlot>();
                        foreach (IceSlot thisSlot in slots)
                        {
                            // Generate a key that is unique for the ice time
                            string slotKey = thisSlot.IceTime.ToString();

                            // Don't want to collapse tournament slots, so make the keys unique
                            TournamentSlot thisTournamentSlot = thisSlot as TournamentSlot;
                            if (thisTournamentSlot != null)
                            {
                                slotKey = thisTournamentSlot.ToString();
                            }

                            // See this ice time is a duplicate
                            IceSlot thatSlot = null;
                            if (!slotMap.TryGetValue(slotKey, out thatSlot))
                            {
                                slotMap[slotKey] = thisSlot;
                                continue;
                            }

                            // Found duplicate slot, so pick the one that is a real game
                            List<IceSlot> slotPair = new List<IceSlot> { thisSlot, thatSlot };
                            slotMap[slotKey] = slotPair.FirstOrDefault(s => s is GameSlot && ((GameSlot)s).Type != GameType.Unknown);

                            if (slotMap[slotKey] == null)
                            {
                                Console.WriteLine("Unable to resolve duplicate:\n{0}\n{1}", thisSlot, thatSlot);
                                PrintUsage();
                                return;
                            }
                        }

                        slots = slotMap.Values.ToList();
                    }
                    else if (processTypeLower == "filter")
                    {
                        Console.WriteLine("Filtering slots using '{0}'.", processArgument);

                        bool invertFilter = false;
                        if (processArgumentLower.StartsWith("!"))
                        {
                            invertFilter = true;
                            processArgumentLower = processArgumentLower.TrimStart(new char[] { '!' });
                            processArgument = processArgument.TrimStart(new char[] { '!' });
                        }

                        if (processArgumentLower == "richmond")
                        {
                            Predicate<IceSlot> filterExpression = (s) => (s is GameSlot && s.ToString().Contains("RichmondGirls"));
                            slots = slots.Where(s => invertFilter ? !filterExpression(s) : filterExpression(s)).ToList();
                        }
                        else if (processArgumentLower == "homegame" || processArgumentLower == "homegames")
                        {
                            Predicate<IceSlot> filterExpression = (s) => (s is GameSlot && (s as GameSlot).HomeTeam.Association == Association.RichmondGirls);
                            slots = slots.Where(s => invertFilter ? !filterExpression(s) : filterExpression(s)).ToList();
                        }
                        else if (processArgumentLower == "awaygame" || processArgumentLower == "awaygames")
                        {
                            // This predicate will keep games where two Ravens teams play each other
                            Predicate<IceSlot> filterExpression = (s) => (s is GameSlot && (s as GameSlot).HomeTeam.Association != Association.RichmondGirls);
                            slots = slots.Where(s => invertFilter ? !filterExpression(s) : filterExpression(s)).ToList();
                        }
                        else if (processArgumentLower == "conflict" || processArgumentLower == "conflicts")
                        {
                            Predicate<IceSlot> filterExpression = (s) => (s is GameSlot && (s as GameSlot).IceTime.Start == DateTime.MinValue);
                            slots = slots.Where(s => invertFilter ? !filterExpression(s) : filterExpression(s)).ToList();
                        }
                        else if (processArgumentLower == "game" || processArgumentLower == "games")
                        {
                            Predicate<IceSlot> filterExpression = (s) => (s is GameSlot);
                            slots = slots.Where(s => invertFilter ? !filterExpression(s) : filterExpression(s)).ToList();
                        }
                        else if (processArgumentLower == "available")
                        {
                            Predicate<IceSlot> filterExpression = (s) => (s is AvailableSlot);
                            slots = slots.Where(s => invertFilter ? !filterExpression(s) : filterExpression(s)).ToList();
                        }
                        else if (processArgumentLower == "skills")
                        {
                            Predicate<IceSlot> filterExpression = (s) => (s is OtherSkillDevelopmentSlot || s is TeamSkillDevelopmentSlot);
                            slots = slots.Where(s => invertFilter ? !filterExpression(s) : filterExpression(s)).ToList();
                        }
                        else if (processArgumentLower == "practice" || processArgumentLower == "practices")
                        {
                            Predicate<IceSlot> filterExpression = (s) => (s is PracticeSlot);
                            slots = slots.Where(s => invertFilter ? !filterExpression(s) : filterExpression(s)).ToList();
                        }
                        else if (processArgumentLower == "tournament" || processArgumentLower == "tournaments")
                        
                        {
                            Predicate<IceSlot> filterExpression = (s) => (s is TournamentSlot);
                            slots = slots.Where(s => invertFilter ? !filterExpression(s) : filterExpression(s)).ToList();
                        }
                        else if (processArgumentLower == "special")
                        {
                            Predicate<IceSlot> filterExpression = (s) => (s is SpecialEventSlot);
                            slots = slots.Where(s => invertFilter ? !filterExpression(s) : filterExpression(s)).ToList();
                        }
                        else if (processArgumentLower.StartsWith("dates"))
                        {
                            string[] dates = processArgument.Split(new[] { ' ' }).Skip(1).ToArray();

                            DateTime startDate = DateTime.Parse(dates[0]);
                            DateTime endDate = startDate;
                            if (dates.Length > 1)
                            {
                                endDate = DateTime.Parse(dates[1]);
                            }

                            slots = slots.Where(s => s.IceTime.Start.Date >= startDate && s.IceTime.Start.Date <= endDate).ToList();
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
                        Dictionary<string, List<IceSlot>> slotMap = finder.FindMultiples(slots);

                        foreach (string key in slotMap.Keys)
                        {
                            List<IceSlot> multipleSlots = slotMap[key];
                            Console.WriteLine("{0}: Found {1} slots on the same day.", key, multipleSlots.Count);
                            multipleSlots.ForEach(s => Console.WriteLine("    " + s.ToString()));
                            Console.WriteLine();
                        }
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
                    SportNginFormatter formatter = new SportNginFormatter();
                    formatter.WriteSchedule(slots, outputPath);
                }
                else if (outputType.ToLower() == "breakdown")
                {
                    SlotBreakdownFormatter formatter = new SlotBreakdownFormatter();
                    formatter.WriteSchedule(slots, outputPath);
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
