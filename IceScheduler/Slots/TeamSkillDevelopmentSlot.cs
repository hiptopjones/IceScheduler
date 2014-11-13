using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public class TeamSkillDevelopmentSlot : TeamBasedIceSlot
    {
        public string Name { get; private set; }
        public List<Team> Teams { get; private set; }

        public TeamSkillDevelopmentSlot(IceTime iceTime, string name, params Team[] teams)
            : base(iceTime)
        {
            Name = name;
            Teams = new List<Team>(teams);
        }

        public override List<Team> GetParticipatingTeams()
        {
            return Teams;
        }

        public override string ToString()
        {
            return string.Format("Skill Development - {0} - {1} - {2}", Name, string.Join("/", Teams), base.ToString());
        }
    }
}
