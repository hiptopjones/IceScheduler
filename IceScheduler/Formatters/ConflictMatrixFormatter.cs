using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Formatters
{
    public class ConflictMatrixFormatter
    {
        public void WriteSchedule(List<IceSlot> slots, string path)
        {
            List<DateTime> dates = GetAllDates(slots);
            List<Team> teams = GetDistinctTeams(slots);

            List<string> rows = new List<string>();

            // Write header row
            List<string> headerFields = new List<string>();

            // Day of week and day fields
            headerFields.Add(string.Empty);
            headerFields.Add(string.Empty);
            foreach (Team team in teams)
            {
                headerFields.Add(string.Format("{0} {1}{2}", team.Association, team.Level, team.Flight));
            }

            rows.Add(string.Join(",", headerFields));

            foreach (DateTime date in dates)
            {
                List<IceSlot> dateSlots = slots.Where(s => (s.IceTime.Start.Date == date)).ToList();

                List<string> fields = new List<string>();

                fields.Add(date.DayOfWeek.ToString());
                fields.Add(date.Day.ToString());

                foreach (Team team in teams)
                {
                    List<IceSlot> teamSlots = dateSlots.Where(s => (s as TeamBasedIceSlot).GetParticipatingTeams().Contains(team)).ToList();
                    if (teamSlots.Any())
                    {
                        fields.Add("X");
                    }
                    else
                    {
                        fields.Add(string.Empty);
                    }
                }

                rows.Add(string.Join(",", fields));
            }

            File.WriteAllText(path, string.Join("\n", rows));
        }

        private List<DateTime> GetAllDates(List<IceSlot> slots)
        {
            DateTime startDate = slots[0].IceTime.Start.Date;
            DateTime endDate = slots[slots.Count - 1].IceTime.Start.Date;

            TimeSpan delta = endDate - startDate;
            int days = (int)delta.TotalDays;

            return Enumerable.Range(0, days).Select(offset => startDate + TimeSpan.FromDays(offset)).ToList();
        }

        private List<Team> GetDistinctTeams(List<IceSlot> slots)
        {
            return slots.SelectMany(s => (s as TeamBasedIceSlot).GetParticipatingTeams()).Distinct().ToList();
        }
    }
}
