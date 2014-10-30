using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IceScheduler.Parsers
{
    public static class ParsingUtilities
    {
        // Parses strings like "6:00-7:00am"
        public static TimeSpan[] ParseTimeRange(string range)
        {
            string[] parts = range.Split(new char[] { '-' });

            if (parts.Length != 2)
            {
                throw new ApplicationException(string.Format("unexpected range formatting: {0}", range));
            }

            DateTime startTime = DateTime.Parse(parts[0]);
            DateTime endTime = DateTime.Parse(parts[1]);

            // If they are more than 12 hours apart, then startTime needs to be moved to PM
            if ((endTime - startTime) > TimeSpan.FromHours(12))
            {
                startTime += TimeSpan.FromHours(12);
            }

            return new TimeSpan[] { startTime.TimeOfDay, endTime.TimeOfDay };
        }

        public static Team ParseTeam(string teamName)
        {
            Regex regex = new Regex(@"(\w+) (\w+) (\w)(\d)");
            Match match = regex.Match(teamName);
            if (!match.Success)
            {
                throw new Exception(string.Format("Unable to parse team name: {0}", teamName));
            }

            Association association = (Association)Enum.Parse(typeof(Association), match.Groups[1].Value);
            Division division = (Division)Enum.Parse(typeof(Division), match.Groups[2].Value);
            Level level = (Level)Enum.Parse(typeof(Level), match.Groups[3].Value);
            int flight = Int32.Parse(match.Groups[4].Value);

            return new Team(association, division, level, flight);
        }

        public static List<Team> ParseRavensTeams(string teamName)
        {
            List<Team> teams = new List<Team>();

            try
            {
                Team team = ParsingUtilities.ParseRavensTeam(teamName);
                teams.Add(team);
            }
            catch (Exception)
            {
                teams = ParsingUtilities.ParseRavensTeamsFromComposite(teamName);
            }

            return teams;
        }

        public static Team ParseRavensTeam(string teamName)
        {
            Regex regex = new Regex(@"(\w+) (\w)(\d)");
            Match match = regex.Match(teamName);
            if (!match.Success)
            {
                throw new Exception(string.Format("Unable to parse team name: {0}", teamName));
            }

            Association association = Association.RichmondGirls;
            Division division = (Division)Enum.Parse(typeof(Division), match.Groups[1].Value);
            Level level = (Level)Enum.Parse(typeof(Level), match.Groups[2].Value);
            int flight = Int32.Parse(match.Groups[3].Value);

            return new Team(association, division, level, flight);
        }

        public static List<Team> ParseRavensTeamsFromComposite(string composite)
        {
            List<Team> teams = new List<Team>();

            string[] parts = composite.Split(new[] { '/' });
            foreach (string part in parts)
            {
                char[] chars = part.ToCharArray();

                Division division = Enum.GetValues(typeof(Division)).OfType<Division>().Where(d => d.ToString()[0] == chars[0]).First();
                Level level = (Level)Enum.Parse(typeof(Level), chars[1].ToString());
                int flight = Int32.Parse(chars[2].ToString());

                teams.Add(new Team(Association.RichmondGirls, division, level, flight));
            }

            return teams;
        }
    }
}
