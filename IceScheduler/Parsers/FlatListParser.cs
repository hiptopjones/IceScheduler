using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;using System.Threading.Tasks;


using System.Text;
using System.Text.RegularExpressions;
namespace IceScheduler.Parsers
{
    public class FlatListParser
    {
        public List<IceSlot> Parse(string path)
        {
            List<IceSlot> schedule = new List<IceSlot>();

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                // DayOfWeek, Date, StartTime, EndTime, Rink, Type, Type-Specific-Fields
                string[] fields = line.Split(new[] { ',' }, StringSplitOptions.None);

                DateTime start = DateTime.Parse(string.Format("{0} {1}", fields[1], fields[2]));
                DateTime end = DateTime.Parse(string.Format("{0} {1}", fields[1], fields[3]));
                Rink rink = (Rink)Enum.Parse(typeof(Rink), fields[4]);

                IceTime iceTime = new IceTime(rink, start, end);
                IceSlot slot = null;

                if (fields[5] == "Available")
                {
                    slot = new AvailableSlot(iceTime);
                }
                else if (fields[5] == "Practice")
                {
                    List<Team> teams = ParseTeamsFromComposite(fields[6]);
                    slot = new PracticeSlot(iceTime, teams.ToArray());
                }
                else if (fields[5] == "Game")
                {
                    GameType type = (GameType)Enum.Parse(typeof(GameType), fields[6]);
                    Team homeTeam = ParseTeam(fields[7]);
                    Team awayTeam = ParseTeam(fields[8]);
                    slot = new GameSlot(iceTime, type, homeTeam, awayTeam);
                }
                else if (fields[5] == "TeamSkills")
                {
                    List<Team> teams = ParseTeamsFromComposite(fields[6]);
                    slot = new TeamSkillDevelopmentSlot(iceTime, teams.ToArray());
                }
                else if (fields[5] == "OtherSkills")
                {
                    slot = new OtherSkillDevelopmentSlot(iceTime, fields[6]);
                }

                schedule.Add(slot);
            }

            return schedule;
        }

        private Team ParseTeam(string teamName)
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

        private List<Team> ParseTeamsFromComposite(string composite)
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
