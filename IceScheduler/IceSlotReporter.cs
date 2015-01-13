using IceScheduler.Slots;
using IceScheduler.Teams;
using IceScheduler.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler
{
    public class IceSlotReporter
    {
        public void ReportTeamSlotBreakdown(List<IceSlot> slots)
        {
            Dictionary<Team, List<IceSlot>> teamSlotMap = SlotUtilities.GetTeamSlotMap(slots);

            foreach (Team team in RavensUtilities.GetRavensTeams())
            {
                List<IceSlot> teamSlots = teamSlotMap[team];

                // Totals (ignore tournament slots)
                TimeSpan totalIceTime = TimeSpan.FromMinutes(teamSlots.Sum(s => (s is TournamentSlot) ? 0 : s.IceTime.Length.TotalMinutes));
                TimeSpan practiceIceTime = TimeSpan.FromMinutes(teamSlots.Sum(s => (s is PracticeSlot) ? s.IceTime.Length.TotalMinutes : 0));
                TimeSpan skillsIceTime = TimeSpan.FromMinutes(teamSlots.Sum(s => (s is TeamSkillDevelopmentSlot) ? s.IceTime.Length.TotalMinutes : 0));
                TimeSpan gameIceTime = TimeSpan.FromMinutes(teamSlots.Sum(s => (s is GameSlot) ? s.IceTime.Length.TotalMinutes : 0));
                
                Console.WriteLine("Slot totals for {0}...", team);
                Console.WriteLine("    Total:    {0} hr", totalIceTime.TotalHours);
                Console.WriteLine("    Practice: {0} hr", practiceIceTime.TotalHours);
                Console.WriteLine("    Skills:   {0} hr", skillsIceTime.TotalHours);
                Console.WriteLine("    Game:     {0} hr", gameIceTime.TotalHours);
                Console.WriteLine();
            }
        }

    }
}
