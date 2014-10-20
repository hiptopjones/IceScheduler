using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Teams
{
    public class Team
    {
        public Association Association { get; private set; }
        public Division Division { get; private set; }
        public Level Level { get; private set; }
        public int Flight { get; private set; }

        public Team(Association association, Division division, Level level, int flight)
        {
            Association = association;
            Division = division;
            Level = level;
            Flight = flight;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}{3}", Association, Division, Level, Flight);
        }
    }
}
