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
                if (string.IsNullOrWhiteSpace(line))
                {
                    // Ignore empty lines
                    continue;
                }

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
                    List<Team> teams = ParsingUtilities.ParseRavensTeams(fields[6]);
                    slot = new PracticeSlot(iceTime, teams.ToArray());
                }
                else if (fields[5] == "Game")
                {
                    GameType type = (GameType)Enum.Parse(typeof(GameType), fields[6]);
                    Team homeTeam = ParsingUtilities.ParseTeam(fields[7]);
                    Team awayTeam = ParsingUtilities.ParseTeam(fields[8]);
                    string otherInfo = fields.Length > 9 ? fields[9] : string.Empty; // Optional field

                    slot = new GameSlot(iceTime, type, homeTeam, awayTeam, otherInfo);
                }
                else if (fields[5] == "TeamSkills")
                {
                    string name = fields[6];
                    List<Team> teams = ParsingUtilities.ParseRavensTeams(fields[7]);
                    slot = new TeamSkillDevelopmentSlot(iceTime, name, teams.ToArray());
                }
                else if (fields[5] == "OtherSkills")
                {
                    slot = new OtherSkillDevelopmentSlot(iceTime, fields[6]);
                }
                else if (fields[5] == "SpecialEvent")
                {
                    string title = fields[6];
                    string subTitle = fields.Length > 7 ? fields[7] : string.Empty; // Optional field

                    slot = new SpecialEventSlot(iceTime, title, subTitle);
                }
                else if (fields[5] == "Tournament")
                {
                    List<Team> teams = ParsingUtilities.ParseRavensTeams(fields[6]);
                    string tounamentName = fields.Length > 7 ? fields[7] : string.Empty; // Optional field

                    slot = new TournamentSlot(iceTime, teams.First(), tounamentName);
                }

                schedule.Add(slot);
            }

            return schedule;
        }
    }
}
