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

        public string ToStringNoAssociation()
        {
            return string.Format("{0} {1}{2}", Division, Level, Flight);
        }

        public override bool Equals(object obj)
        {
            Team other = obj as Team;
            if (other == null)
            {
                return false;
            }

            return other.Association == Association && other.Division == Division && other.Level == Level && other.Flight == Flight;
        }

        public override int GetHashCode()
        {
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 486187739 + Association.GetHashCode();
                hash = hash * 486187739 + Division.GetHashCode();
                hash = hash * 486187739 + Level.GetHashCode();
                hash = hash * 486187739 + Flight.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Association, ToStringNoAssociation());
        }
    }
}
