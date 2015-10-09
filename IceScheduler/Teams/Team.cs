using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public string ToStringNoAssociation()
        {
            return string.Format("{0} {1}{2}", Division, Level, Flight);
        }

        public string ToStringVersus()
        {
            if (!AssociationVersusMap.ContainsKey(Association))
            {
                throw new Exception(string.Format("Unrecognized association: {0}", Association));
            }

            string shortName = AssociationVersusMap[Association];
            return string.Format("{0} {1}{2}", shortName, Level, Flight);
        }

        public static Team FromVersusString(string input, Division division = Division.Tyke)
        {
            Regex regex = new Regex(@"^(.*)\s+(C|A)(\d+)$");
            Match match = regex.Match(input);
            if (match.Success)
            {
                var reverseMap = AssociationVersusMap.ToLookup(pair => pair.Value, pair => pair.Key);

                Association association = reverseMap[match.Groups[1].Value].First();
                Level level = (Level)Enum.Parse(typeof(Level), match.Groups[2].Value);
                int flight = Int32.Parse(match.Groups[3].Value);

                return new Team(association, division, level, flight);
            }

            throw new Exception(string.Format("Unable to parse into Team: '{0}'", input));
        }

        // This should probably go in an Association class
        private static Dictionary<Association, string> AssociationVersusMap = new Dictionary<Association, string>
        {
            { Association.AbbotsfordFemale, "Abbotsford" },
            { Association.BurnabyFemale, "Burnaby" },
            { Association.ChilliwackFemale, "Chilliwack" },
            { Association.LangleyGirls, "Langley" },
            { Association.MeadowRidgeFemale, "M Ridge" },
            { Association.NorthShoreFemale, "N Shore" },
            { Association.NorthShoreWinterClubFemale, "NSWC" },
            { Association.RichmondGirls, "Richmond" },
            { Association.SouthDeltaFemale, "S Delta" },
            { Association.SurreyFemale, "Surrey" },
            { Association.TriCitiesFemale, "Tri-Cities" },
            { Association.VancouverGirls, "Vancouver" },
            { Association.WesternWashingtonFemale, "W Washington" },
            { Association.Unknown, "Unknown" },
        };
    }
}
