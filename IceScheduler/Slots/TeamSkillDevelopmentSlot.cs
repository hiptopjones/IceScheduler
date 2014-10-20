using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public class TeamSkillDevelopmentSlot : IceSlot
    {
        public List<Team> Teams { get; private set; }

        public TeamSkillDevelopmentSlot(IceTime iceTime, params Team[] teams)
            : base(iceTime)
        {
            Teams = new List<Team>(teams);
        }

        public override string ToString()
        {
            return string.Format("Skill Development - {0} - {1}", string.Join("/", Teams), base.ToString());
        }
    }
}
