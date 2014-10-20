using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public class OtherSkillDevelopmentSlot : IceSlot
    {
        public string Name { get; private set; }

        public OtherSkillDevelopmentSlot(IceTime iceTime, string name)
            : base(iceTime)
        {
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("Skill Development - {0} - {1}", Name, base.ToString());
        }
    }
}
