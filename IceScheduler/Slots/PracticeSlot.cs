using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public class PracticeSlot : TeamBasedIceSlot
    {
        public List<Team> Teams { get; private set; }

        public PracticeSlot(IceTime iceTime, params Team[] teams)
            : base(iceTime)
        {
            Teams = new List<Team>(teams);
        }

        public override List<Team> GetParticipatingTeams()
        {
            return Teams;
        }

        public override string ToString()
        {
            return string.Format("Practice - {0} - {1}", string.Join("/", Teams), base.ToString());
        }
    }
}
