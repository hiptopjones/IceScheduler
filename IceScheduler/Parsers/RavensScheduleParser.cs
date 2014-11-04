using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IceScheduler.Parsers
{
    // TODO: This is super-rough code, need to refactor and UT
    public class RavensScheduleParser
    {
        // Parses the input schedule in a CSV format
        public List<IceSlot> Parse(string path)
        {
            List<IceSlot> schedule = new List<IceSlot>();
            bool isInSchedule = false;

            // Initialize day-of-week lists
            List<string>[] dayOfWeekLists = new List<string>[7];
            for (int i = 0; i < dayOfWeekLists.Length; i++)
            {
                dayOfWeekLists[i] = new List<string>();
            }

            // Parse CSV into day-of-week lists
            foreach (string line in File.ReadAllLines(path))
            {
                // Do not remove empty fields
                string[] fields = line.Split(new[] { ',' }, StringSplitOptions.None);

                // Monday line marks the start and end of the schedule body
                if (fields[1].Contains("Monday"))
                {
                    if (isInSchedule)
                    {
                        // End of schedule body
                        break;
                    }
                    else
                    {
                        // Start of schedule body
                        isInSchedule = true;
                        continue;
                    }
                }

                // Don't do anything if we're not in the schedule body yet
                if (!isInSchedule)
                {
                    continue;
                }

                // Add the field values to the appropriate day-of-week lists
                for (int i = 0; i < dayOfWeekLists.Length; i++)
                {
                    // Must +1 because the first column contains time scale, and should be ignored
                    dayOfWeekLists[i].Add(fields[i + 1]);
                }
            }

            // Parse the day-of-week lists
            for (int i = 0; i < dayOfWeekLists.Length; i++)
            {
                List<string> dayOfWeekEntries = dayOfWeekLists[i];
                for (int j = 0; j < dayOfWeekEntries.Count; j++)
                {
                    // Matches ranges like "9:30-10:45am"
                    Regex timeRange = new Regex(@"^(\d+:\d+)-(\d+:\d+[ap]m)");
                    Match match = timeRange.Match(dayOfWeekEntries[j]);
                    if (match.Success)
                    {
                        TimeSpan[] times = ParsingUtilities.ParseTimeRange(dayOfWeekEntries[j]);
                        TimeSpan startTime = times[0];
                        TimeSpan endTime = times[1];

                        string header = dayOfWeekEntries[j - 1];
                        Rink rink = ParseRink(dayOfWeekEntries[j + 1]);
                        string footer = dayOfWeekEntries[j + 2];

                        // Need to do +1 because these schedules are indexed Monday to Sunday, but the DayOfWeek is indexed Sunday to Saturday
                        DayOfWeek dayOfWeek = (DayOfWeek)((i + 1) % 7);
                        IceTime iceTime = new IceTime(rink, dayOfWeek, startTime, endTime);

                        IceSlot slot = null;

                        if (header == "Development")
                        {
                            if (footer == "Goalies" || footer == "Various")
                            {
                                slot = new OtherSkillDevelopmentSlot(iceTime, footer);
                            }
                            else
                            {
                                List<Team> teams = ParsingUtilities.ParseRavensTeams(footer);
                                slot = new TeamSkillDevelopmentSlot(iceTime, header, teams.ToArray());
                            }
                        }
                        else if (header == "Available" || header == "Conflict Ice")
                        {
                            slot = new AvailableSlot(iceTime);
                        }
                        else if (header == "Skill Accelerator")
                        {
                            List<Team> teams = ParsingUtilities.ParseRavensTeams(footer);
                            slot = new TeamSkillDevelopmentSlot(iceTime, header, teams.ToArray());
                        }
                        else
                        {
                            // Try to parse the team name
                            List<Team> teams = ParsingUtilities.ParseRavensTeams(header);
                            slot = new PracticeSlot(iceTime, teams.ToArray());
                        }

                        if (slot != null)
                        {
                            schedule.Add(slot);
                        }
                    }
                }
            }

            return schedule;
        }

        private Rink ParseRink(string rinkName)
        {
            switch (rinkName)
            {
                case "Stadium":
                    return Rink.Stadium;
                case "Silver":
                    return Rink.Silver;
                case "Oval North":
                    return Rink.OvalNorth;
                case "Oval South":
                    return Rink.OvalSouth;
                case "Igloo":
                    return Rink.Igloo;
                case "Garage":
                    return Rink.Garage;
                case "Pond":
                    return Rink.Pond;
                case "Coliseum":
                    return Rink.Coliseum;
                case "Forum":
                    return Rink.Forum;
                case "Gardens":
                    return Rink.Gardens;
                default:
                    return Rink.Unknown;
            }
        }
    }
}
