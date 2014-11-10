using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public class TournamentSlot : IceSlot
    {
        public Team Team { get; private set; }
        public string Name { get; private set; }

        public TournamentSlot(IceTime iceTime, Team team, string name)
            : base(iceTime)
        {
            Team = team;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("Tournament - {0} - {1} - {2}", Team, Name, base.ToString());
        }
    }
}
