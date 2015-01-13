using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Utilities
{
    public class SlotUtilities
    {
        public static Dictionary<Team, List<IceSlot>> GetTeamSlotMap(List<IceSlot> slots)
        {
            Dictionary<Team, List<IceSlot>> teamSlotMap = new Dictionary<Team, List<IceSlot>>();
            slots.ForEach(s =>
            {
                List<Team> teams = GetTeamsFromIceSlot(s);
                foreach (Team team in teams)
                {
                    List<IceSlot> teamSlots;
                    if (!teamSlotMap.TryGetValue(team, out teamSlots))
                    {
                        teamSlots = new List<IceSlot>();
                        teamSlotMap[team] = teamSlots;
                    }

                    teamSlots.Add(s);
                }
            });

            return teamSlotMap;
        }

        public static List<Team> GetTeamsFromIceSlot(IceSlot slot)
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
