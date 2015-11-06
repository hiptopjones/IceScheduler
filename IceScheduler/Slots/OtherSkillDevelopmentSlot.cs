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
        public string OtherInfo { get; private set; }

        public OtherSkillDevelopmentSlot(IceTime iceTime, string name, string otherInfo)
            : base(iceTime)
        {
            Name = name;
            OtherInfo = otherInfo;
        }

        public override string ToString()
        {
            string otherInfo = string.IsNullOrEmpty(OtherInfo) ? string.Empty : string.Format(" ({0})", OtherInfo);
            return string.Format("Skill Development - {0}{1} - {2}", Name, otherInfo, base.ToString());
        }
    }
}
