using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IceScheduler.Parsers
{
    public static class PcahaTeamParser
    {
        public static Team ParseIntoTeam(string teamName)
        {
            // <Association> <Division> <Level><Flight>
            // eg. Western Washington Female Midget C1

            if (teamName.EndsWith("Juvenile"))
            {
                teamName += " C1";
            }

            string[] teamNameParts = teamName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string levelAndFlight = teamNameParts[teamNameParts.Length - 1];
            string divisionName = teamNameParts[teamNameParts.Length - 2];
            string associationName = string.Join(" ", teamNameParts, 0, teamNameParts.Length - 2);

            Association association = PcahaAssociationMap[associationName];
            Division division = PcahaDivisionMap[divisionName];

            // Parsing the "C1"
            Regex regex = new Regex(@"^([A-Z]+)(\d+)?");
            Match match = regex.Match(levelAndFlight);
            if (!match.Success)
            {
                throw new Exception(string.Format("Unable to parse level and flight: {0}", levelAndFlight));
            }

            Level level = PcahaLevelMap[match.Groups[1].Value];

            // Flight may be omitted on some teams
            int flight = 1;
            if (!string.IsNullOrEmpty(match.Groups[2].Value))
            {
                flight = Int32.Parse(match.Groups[2].Value);
            }

            return new Team(association, division, level, flight);
        }

        private static Dictionary<string, Level> PcahaLevelMap = new Dictionary<string, Level>
        {
            {"A", Level.A},
            {"C", Level.C},
        };

        private static Dictionary<string, Division> PcahaDivisionMap = new Dictionary<string, Division>
        {
            {"Atom", Division.Atom},
            {"Bantam", Division.Bantam},
            {"Juvenile", Division.Juvenile},
            {"Midget", Division.Midget},
            {"Novice", Division.Novice},
            {"Peewee", Division.Peewee},
            {"Tyke", Division.Tyke},
        };

        private static Dictionary<string, Association> PcahaAssociationMap = new Dictionary<string, Association>
        {
            {"Abbotsford Female", Association.AbbotsfordFemale},
            {"Burnaby Female", Association.BurnabyFemale},
            {"Chilliwack Female", Association.ChilliwackFemale},
            {"Langley Girls", Association.LangleyGirls},
            {"Meadow Ridge Female", Association.MeadowRidgeFemale},
            {"North Shore Female", Association.NorthShoreFemale},
            {"North Shore Winter Club Female", Association.NorthShoreWinterClubFemale},
            {"Richmond Girls", Association.RichmondGirls},
            {"South Delta Female", Association.SouthDeltaFemale},
            {"Surrey Female", Association.SurreyFemale},
            {"Tri-Cities Female", Association.TriCitiesFemale},
            {"Tri Cities Female", Association.TriCitiesFemale},
            {"Vancouver Girls", Association.VancouverGirls},
            {"Western Washington Female", Association.WesternWashingtonFemale},
        };
    }
}
