﻿using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Formatters
{
    public class RavensScheduleFormatter
    {
        public void WriteSchedule(List<IceSlot> slots, string path)
        {
            // Must only be a schedule for one week (Monday to Sunday)
            if (slots.Last().IceTime.End.Date - slots.First().IceTime.Start.Date > TimeSpan.FromDays(7))
            {
                throw new Exception("The Ravens schedule formatter only supports formatting a week at a time.");
            }

            // Must start on Monday and end on Sunday
            if (slots.First().IceTime.Start.DayOfWeek != DayOfWeek.Monday || slots.Last().IceTime.End.DayOfWeek != DayOfWeek.Sunday)
            {
                throw new Exception("The Ravens schedule formatter only supports schedules that start on Monday and end on Sunday.");
            }

            TimeSpan startOfDay = TimeSpan.FromHours(6);
            TimeSpan endOfDay = TimeSpan.FromHours(22);
            int numRows = (int)((endOfDay - startOfDay).TotalHours * 4);
            
            // Initialize the day-of-week lists and rows
            string[][] dayOfWeekLists = new string[7][];
            for (int i = 0; i < dayOfWeekLists.Length; i++)
            {
                dayOfWeekLists[i] = new string[numRows];
            }

            // Fit the formatted slots in the day-of-week columns
            //  1. Create a tree of rows (one for each quarter hour), indexed by time
            //  2. Add each entry to its row, slipping down if necessary
            //  3. Once all entries are added, determine if the last entry is falling off the bottom
            //  4. If so, find the last empty row, and slide everything up into it.
            //  5. Repeat 3 and 4 until everything fits or we're out of room (exception)

            int numRowsPerSlot = 4;

            foreach (IceSlot slot in slots)
            {
                string[] dayOfWeekRows = dayOfWeekLists[(int)slot.IceTime.Start.DayOfWeek];

                GameSlot gameSlot = slot as GameSlot;
                if (gameSlot != null)
                {
                    if (gameSlot.HomeTeam.Association != Association.RichmondGirls)
                    {
                        // TODO: Handle away game slots specially
                        continue;
                    }
                }

                TimeSpan timeOfDay = slot.IceTime.Start.TimeOfDay;
                int rowIndex = GetRowIndex(startOfDay, timeOfDay);

                // Find the next available row
                while (rowIndex < dayOfWeekRows.Length && dayOfWeekRows[rowIndex] != null)
                {
                    rowIndex++;
                }

                // If we run into the bottom of the schedule, slide things up to make it fit
                int remainingRows = dayOfWeekRows.Length - rowIndex;
                if (remainingRows < numRowsPerSlot)
                {
                    SlideEventsUp(dayOfWeekRows, numRowsPerSlot - remainingRows);
                    rowIndex = dayOfWeekRows.Length - numRowsPerSlot - 1;
                }

                dayOfWeekRows[rowIndex] = GetHeader(slot);
                dayOfWeekRows[rowIndex + 1] = GetTime(slot);
                dayOfWeekRows[rowIndex + 2] = GetRink(slot);
                dayOfWeekRows[rowIndex + 3] = GetFooter(slot);
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<html>");
            builder.AppendLine("<head>");
            builder.AppendLine("<title> Weekly Schedule </title>");
            builder.AppendLine(GetStyleSheet());
            builder.AppendLine("</head>");
            builder.AppendLine("<body>");
            builder.AppendLine("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=743 style='border-collapse: collapse;table-layout:fixed'>");

            builder.AppendLine("<tr>");
            builder.AppendLine(GetTableCell("DayOfWeek", string.Empty));
            foreach (DayOfWeek dayOfWeek in GetDaysOfWeek())
            {
                builder.AppendLine(GetTableCell("DayOfWeek", dayOfWeek.ToString()));
            }
            builder.AppendLine("</tr>");

            TimeSpan interval = TimeSpan.FromMinutes(15);
            TimeSpan current = startOfDay;
            for (int i = 0; i < numRows; i++)
            {
                builder.AppendLine("<tr>");
                if ((i % 4) == 0)
                {
                    builder.AppendLine(GetTableCell("TimeScale RightEdge", current.ToString(@"hh\:mm")));
                }
                else
                {
                    builder.AppendLine(GetTableCell("RightEdge", string.Empty));
                }

                foreach (DayOfWeek dayOfWeek in GetDaysOfWeek())
                {
                    builder.AppendLine(dayOfWeekLists[(int)dayOfWeek][i] ?? GetTableCell(string.Empty));
                }
                builder.AppendLine("</tr>");

                current += interval;
            }

            builder.AppendLine("<tr>");
            builder.AppendLine(GetTableCell("DayOfWeek", string.Empty));
            foreach (DayOfWeek dayOfWeek in GetDaysOfWeek())
            {
                builder.AppendLine(GetTableCell("DayOfWeek", dayOfWeek.ToString()));
            }
            builder.AppendLine("</tr>");

            builder.AppendLine("</table>");
            builder.AppendLine("</body>");
            builder.AppendLine("</html>");

            // Dump the HTML out to the specified file
            File.WriteAllText(path, builder.ToString());
        }

        private string GetHeader(IceSlot slot)
        {
            if (slot is AvailableSlot)
            {
                return GetTableCell("AvailableHeader", "Available");
            }
            else if (slot is PracticeSlot)
            {
                PracticeSlot practiceSlot = slot as PracticeSlot;
                return GetTableCell("PracticeHeader", FormattingUtilities.GetCompositeTeamName(practiceSlot.Teams));
            }
            else if (slot is TeamSkillDevelopmentSlot)
            {
                TeamSkillDevelopmentSlot skillsSlot = slot as TeamSkillDevelopmentSlot;
                return GetTableCell("SkillsHeader", skillsSlot.Name);
            }
            else if (slot is OtherSkillDevelopmentSlot)
            {
                return GetTableCell("SkillsHeader", "Development");
            }
            else if (slot is GameSlot)
            {
                GameSlot gameSlot = slot as GameSlot;
                Team homeTeam = gameSlot.HomeTeam;
                return GetTableCell("GameHeader", homeTeam.ToStringNoAssociation());
            }
            else
            {
                throw new Exception(string.Format("Unhandled slot type: {0}", slot));
            }
        }

        private string GetTime(IceSlot slot)
        {
            string timeRange = FormattingUtilities.GetTimeRange(slot.IceTime);

            if (slot is AvailableSlot)
            {
                return GetTableCell("NormalTime", timeRange);
            }
            else if (slot is PracticeSlot)
            {
                return GetTableCell("NormalTime", timeRange);
            }
            else if (slot is TeamSkillDevelopmentSlot)
            {
                return GetTableCell("SkillsTime", timeRange);
            }
            else if (slot is OtherSkillDevelopmentSlot)
            {
                return GetTableCell("SkillsTime", timeRange);
            }
            else if (slot is GameSlot)
            {
                return GetTableCell("NormalTime", timeRange);
            }
            else
            {
                throw new Exception(string.Format("Unhandled slot type: {0}", slot));
            }
        }

        private string GetRink(IceSlot slot)
        {
            if (slot is AvailableSlot)
            {
                return GetTableCell("NormalRink", GetRinkName(slot.IceTime.Rink));
            }
            else if (slot is PracticeSlot)
            {
                return GetTableCell("NormalRink", GetRinkName(slot.IceTime.Rink));
            }
            else if (slot is TeamSkillDevelopmentSlot)
            {
                return GetTableCell("SkillsRink", GetRinkName(slot.IceTime.Rink));
            }
            else if (slot is OtherSkillDevelopmentSlot)
            {
                return GetTableCell("SkillsRink", GetRinkName(slot.IceTime.Rink));
            }
            else if (slot is GameSlot)
            {
                return GetTableCell("NormalRink", GetRinkName(slot.IceTime.Rink));
            }
            else
            {
                throw new Exception(string.Format("Unhandled slot type: {0}", slot));
            }
        }

        private string GetFooter(IceSlot slot)
        {
            if (slot is AvailableSlot)
            {
                return GetTableCell("NormalFooter", string.Empty);
            }
            else if (slot is PracticeSlot)
            {
                PracticeSlot practiceSlot = slot as PracticeSlot;
                return GetTableCell("NormalFooter", string.Empty);
            }
            else if (slot is TeamSkillDevelopmentSlot)
            {
                TeamSkillDevelopmentSlot skillsSlot = slot as TeamSkillDevelopmentSlot;
                return GetTableCell("SkillsFooter", FormattingUtilities.GetCompositeTeamName(skillsSlot.Teams));
            }
            else if (slot is OtherSkillDevelopmentSlot)
            {
                OtherSkillDevelopmentSlot skillsSlot = slot as OtherSkillDevelopmentSlot;
                return GetTableCell("SkillsFooter", skillsSlot.Name);
            }
            else if (slot is GameSlot)
            {
                GameSlot gameSlot = slot as GameSlot;
                Team awayTeam = gameSlot.AwayTeam;
                return GetTableCell("GameFooter", string.Format("v. {0}", GetVersusName(awayTeam)));
            }
            else
            {
                throw new Exception(string.Format("Unhandled slot type: {0}", slot));
            }
        }

        private string GetTableCell(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = "&nbsp;";
            }

            return string.Format("<td class=\"EmptyCell\">{0}</td>", text);
        }

        private string GetTableCell(string className, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = "&nbsp;";
            }

            return string.Format("<td class=\"{0}\">{1}</td>", className, text);
        }

        private string GetVersusName(Team team)
        {
            switch (team.Association)
            {
                case Association.AbbotsfordFemale:
                    return "Abbotsford";
                case Association.BurnabyFemale:
                    return "Burnaby";
                case Association.ChilliwackFemale:
                    return "Chilliwack";
                case Association.LangleyGirls:
                    return "Langley";
                case Association.MeadowRidgeFemale:
                    return "M Ridge";
                case Association.NorthShoreFemale:
                    return "N Shore";
                case Association.NorthShoreWinterClubFemale:
                    return "NSWC";
                case Association.RichmondGirls:
                    return team.ToStringNoAssociation();
                case Association.SouthDeltaFemale:
                    return "S Delta";
                case Association.SurreyFemale:
                    return "Surrey";
                case Association.TriCitiesFemale:
                    return "Tri-Cities";
                case Association.VancouverGirls:
                    return "Vancouver";
                case Association.WesternWashingtonFemale:
                    return "W Washington";
                default:
                    return "Unknown";
            }
        }

        private string GetRinkName(Rink rink)
        {
            switch (rink)
            {
                case Rink.Stadium:
                    return "Stadium";
                case Rink.Silver:
                    return "Silver";
                case Rink.OvalNorth:
                    return "Oval North";
                case Rink.OvalSouth:
                    return "Oval South";
                case Rink.Igloo:
                    return "Igloo";
                case Rink.Garage:
                    return "Garage";
                case Rink.Pond:
                    return "Pond";
                case Rink.Coliseum:
                    return "Coliseum";
                case Rink.Forum:
                    return "Forum";
                case Rink.Gardens:
                    return "Gardens";
                default:
                    return "Unknown";
            }
        }
        
        private List<DayOfWeek> GetDaysOfWeek()
        {
            return new List<DayOfWeek>
            {
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            };
        }

        private int GetRowIndex(TimeSpan startTime, TimeSpan actualTime)
        {
            return (int)((actualTime - startTime).TotalMinutes / 15);
        }

        private void SlideEventsUp(string[] rows, int numItemsToSlide)
        {
            for (int i = rows.Length - 1; i >= 0; i--)
            {
                if (numItemsToSlide == 0)
                { 
                    break; 
                }

                if (rows[i] == null)
                {
                    numItemsToSlide--;

                    // Now that a row has been found, copy the array elements up.
                    for (int j = i; j < rows.Length - 1; j++)
                    {
                        rows[j] = rows[j + 1];
                    }
                }
            }

            if (numItemsToSlide > 0)
            {
                throw new Exception("Unable to fit the schedule.");
            }
        }

        private string GetStyleSheet()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<style TYPE=\"text/css\">");
            builder.AppendLine("<!--");
            builder.AppendLine(GetStyleSheetEmbeddedResource());
            builder.AppendLine("-->");
            builder.AppendLine("</style");

            return builder.ToString();
        }

        private string GetStyleSheetEmbeddedResource()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("IceScheduler.Formatters.RavensStyleSheet.css")))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
