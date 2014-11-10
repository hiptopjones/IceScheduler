using IceScheduler.Slots;
using IceScheduler.Teams;
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
                List<Team> teams = GetTeamsFromIceSlot(s);
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

        private List<Team> GetTeamsFromIceSlot(IceSlot slot)
        {
            List<Team> teams = new List<Team>();

            if (slot is GameSlot)
            {
                GameSlot gameSlot = slot as GameSlot;
                teams.Add(gameSlot.HomeTeam);
                teams.Add(gameSlot.AwayTeam);
            }
            else if (slot is PracticeSlot)
            {
                PracticeSlot practiceSlot = slot as PracticeSlot;
                teams.AddRange(practiceSlot.Teams);
            }
            else if (slot is TeamSkillDevelopmentSlot)
            {
                TeamSkillDevelopmentSlot skillSlot = slot as TeamSkillDevelopmentSlot;
                teams.AddRange(skillSlot.Teams);
            }
            else if (slot is TournamentSlot)
            {
                TournamentSlot tournamentSlot = slot as TournamentSlot;
                teams.Add(tournamentSlot.Team);
            }

            return teams;
        }
    }
}
