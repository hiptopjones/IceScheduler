﻿using IceScheduler.Slots;
using IceScheduler.Teams;
using IceScheduler.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Formatters
{
    public class SlotBreakdownFormatter
    {
        public void WriteSchedule(List<IceSlot> slots, string path)
        {
            if (!slots.Any())
            {
                // Can't write a schedule if there's nothing to show
                return;
            }

            Dictionary<Team, List<IceSlot>> teamSlotMap = SlotUtilities.GetTeamSlotMap(slots);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<html>");
            builder.AppendLine("<head>");
            builder.AppendLine("<title>Slot Breakdown</title>");
            builder.AppendLine(GetStyleSheet());
            builder.AppendLine("</head>");
            builder.AppendLine("<body>");
            builder.AppendLine("<table border=\"1\" cellpadding=\"0\" cellspacing=\"0\" width=1200 style='border-collapse: collapse'>");

            // Title row
            builder.AppendLine("<tr>");
            builder.AppendLine(GetTableCell("Title", "Slot Breakdown", "colspan=\"9\""));
            builder.AppendLine("</tr>");

            // Blank row
            builder.AppendLine("<tr>");
            builder.AppendLine(GetTableCell(string.Empty));
            builder.AppendLine("</tr>");

            builder.AppendLine("<tr>");
            builder.AppendLine(GetTableCell(string.Empty));
            builder.AppendLine(GetTableCell(string.Empty, "Practices", "colspan=\"2\""));
            builder.AppendLine(GetTableCell(string.Empty, "Skills", "colspan=\"2\""));
            builder.AppendLine(GetTableCell(string.Empty, "Games", "colspan=\"2\""));
            builder.AppendLine(GetTableCell(string.Empty, "Totals", "colspan=\"2\""));
            builder.AppendLine("</tr>");

            builder.AppendLine("<tr>");
            builder.AppendLine(GetTableCell("Team"));
            builder.AppendLine(GetTableCell("Count"));
            builder.AppendLine(GetTableCell("Hours"));
            builder.AppendLine(GetTableCell("Count"));
            builder.AppendLine(GetTableCell("Hours"));
            builder.AppendLine(GetTableCell("Count"));
            builder.AppendLine(GetTableCell("Hours"));
            builder.AppendLine(GetTableCell("Count"));
            builder.AppendLine(GetTableCell("Hours"));
            builder.AppendLine("</tr>");

            // HACK: Use a hard-coded list of teams to enable proper sorting
            foreach (Team team in RavensUtilities.GetRavensTeams())
            {
                List<IceSlot> teamSlots;
                if (!teamSlotMap.TryGetValue(team, out teamSlots))
                {
                    teamSlots = new List<IceSlot>();
                }

                // Totals (ignore tournament slots)
                TimeSpan practiceIceTime = TimeSpan.FromMinutes(teamSlots.Sum(s => (s is PracticeSlot) ? s.IceTime.Length.TotalMinutes : 0));
                int practiceCount = teamSlots.Count(s => (s is PracticeSlot));
                TimeSpan skillsIceTime = TimeSpan.FromMinutes(teamSlots.Sum(s => (s is TeamSkillDevelopmentSlot) ? s.IceTime.Length.TotalMinutes : 0));
                int skillsCount = teamSlots.Count(s => (s is TeamSkillDevelopmentSlot));
                TimeSpan gameIceTime = TimeSpan.FromMinutes(teamSlots.Sum(s => (s is GameSlot) ? s.IceTime.Length.TotalMinutes : 0));
                int gameCount = teamSlots.Count(s => (s is GameSlot));
                TimeSpan totalIceTime = TimeSpan.FromMinutes(teamSlots.Sum(s => (s is TournamentSlot) ? 0 : s.IceTime.Length.TotalMinutes));
                int totalCount = teamSlots.Count(s => !(s is TournamentSlot));

                builder.AppendLine("<tr>");
                builder.AppendLine(GetTableCell(team.ToString()));
                builder.AppendLine(GetTableCell(practiceCount.ToString()));
                builder.AppendLine(GetTableCell(practiceIceTime.TotalHours.ToString()));
                builder.AppendLine(GetTableCell(skillsCount.ToString()));
                builder.AppendLine(GetTableCell(skillsIceTime.TotalHours.ToString()));
                builder.AppendLine(GetTableCell(gameCount.ToString()));
                builder.AppendLine(GetTableCell(gameIceTime.TotalHours.ToString()));
                builder.AppendLine(GetTableCell(totalCount.ToString()));
                builder.AppendLine(GetTableCell(totalIceTime.TotalHours.ToString()));
                builder.AppendLine("</tr>");
            }

            builder.AppendLine("</table>");
            builder.AppendLine("</body>");
            builder.AppendLine("</html>");

            // Dump the HTML out to the specified file
            File.WriteAllText(path, builder.ToString());
        }

        private string GetTableCell(string text)
        {
            return GetTableCell(string.Empty, text);
        }

        private string GetTableCell(string className, string text)
        {
            return GetTableCell(className, text, string.Empty);
        }

        private string GetTableCell(string className, string text, string attributes)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = "&nbsp;";
            }

            return string.Format("<td class=\"{0}\" {1}>{2}</td>", className, attributes, text);
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
