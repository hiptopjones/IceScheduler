using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public abstract class TeamBasedIceSlot : IceSlot
    {
        public TeamBasedIceSlot(IceTime iceTime)
            : base(iceTime)
        {

        }

        public bool HasParticipatingTeam(Team team)
        {
            return GetParticipatingTeams().Contains(team);
        }

        public abstract List<Team> GetParticipatingTeams();
    }
}
