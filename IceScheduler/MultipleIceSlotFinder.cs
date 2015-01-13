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
    public class MultipleIceSlotFinder
    {
        public List<IceSlot> FindMultiples(List<IceSlot> slots)
        {
            Dictionary<string, List<IceSlot>> teamAndDateMap = new Dictionary<string, List<IceSlot>>();
            slots.ForEach(s =>
            {
                DateTime date = s.IceTime.Start.Date;
                List<Team> teams = SlotUtilities.GetTeamsFromIceSlot(s);
                foreach (Team team in teams)
                {
                    string teamAndDate = string.Format("{0} / {1}", team.ToString(), date.ToShortDateString());

                    List<IceSlot> teamAndDateSlots;
                    if (!teamAndDateMap.TryGetValue(teamAndDate, out teamAndDateSlots))
                    {
                        teamAndDateSlots = new List<IceSlot>();
                        teamAndDateMap[teamAndDate] = teamAndDateSlots;
                    }

                    teamAndDateSlots.Add(s);
                }
            });

            return teamAndDateMap.SelectMany(p =>
            {
                if (p.Value.Count > 1)
                {
                    return p.Value;
                }

                return new List<IceSlot>();
            }).ToList();
        }
    }
}
