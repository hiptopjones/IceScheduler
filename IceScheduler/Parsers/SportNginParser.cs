using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IceScheduler.Parsers
{
    public class SportNginParser
    {
        // Field indexes
        private struct Field
        {
            public const int StartDateTime = 1;
            public const int EndDateTime = 2;
            public const int Title = 3;
            public const int Location = 5;
            public const int EventType = 8;
            public const int Tags = 9;
            public const int Team1 = 10;
            public const int Team1IsHome = 11;
            public const int Team2 = 12;
            public const int Team2Name = 13;
            public const int EventId = 14;
        }

        // Parses the input schedule in a CSV format
        public List<IceSlot> Parse(string path)
        {
            List<IceSlot> schedule = new List<IceSlot>();

            foreach (string line in File.ReadAllLines(path))
            {
                // Ignore header
                if (line.Contains("Start_Date_Time"))
                {
                    continue;
                }

                // Hack to work around multi-line descriptions in events
                if (line.Contains(",Game,"))
                {
                    IceSlot slot = ParseIntoIceSlot(line);
                    if (slot != null)
                    {
                        schedule.Add(slot);
                    }
                }
            }

            return schedule;
        }

        private IceSlot ParseIntoIceSlot(string line)
        {
            IceSlot slot = null;

            string[] fields = ParseFieldsFromCsvLine(line);

            Rink rink = ParseRink(fields[Field.Location]);
            DateTime startTime = DateTime.Parse(fields[Field.StartDateTime]);
            DateTime endTime = DateTime.Parse(fields[Field.EndDateTime]);

            IceTime iceTime = new IceTime(rink, startTime, endTime);

            if (fields[Field.EventType] == "Game")
            {
                Team team1 = ParseTeamFromTag(fields[Field.Team1]);
                Team team2 = ParseTeamFromTag(fields[Field.Team2Name]);
                bool isTeam1Home = fields[Field.Team1IsHome] == "1" ? true : false;
                slot = new GameSlot(iceTime, GameType.Tiering, isTeam1Home ? team1 : team2, isTeam1Home ? team2 : team1);
            }
            else
            {
                // Not parsing anything else
            }

            return slot;
        }

        private string[] ParseFieldsFromCsvLine(string line)
        {
            List<string> fields = new List<string>();

            int startIndex = 0;
            int endIndex = 0;
            while (startIndex < (line.Length - 1))
            {
                string field = null;

                // Check if field starts with a quote
                if (line.IndexOf('"', startIndex) == startIndex)
                {
                    startIndex++;
                    endIndex = line.IndexOf('"', startIndex);
                    field = line.Substring(startIndex, endIndex - startIndex);
                    endIndex++;
                }
                else
                {
                    endIndex = line.IndexOf(',', startIndex);
                    if (endIndex < 0)
                    {
                        endIndex = line.Length - 1;
                    }

                    field = line.Substring(startIndex, endIndex - startIndex);
                }

                fields.Add(field);
                startIndex = endIndex + 1;
            }

            return fields.ToArray();
        }
        private Team ParseTeamFromTag(string tag)
        {
            Regex regex = new Regex(@"^(\w+)(C|A)(\d+)$");
            Match match = regex.Match(tag);
            if (match.Success)
            {
                Division division = (Division)Enum.Parse(typeof(Division), match.Groups[1].Value);
                Level level = (Level)Enum.Parse(typeof(Level), match.Groups[2].Value);
                int flight = Int32.Parse(match.Groups[3].Value);

                return new Team(Association.RichmondGirls, division, level, flight);
            }

            try
            {
                return Team.FromVersusString(tag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Team(Association.Unknown, Division.Tyke, Level.C, 1);
            }
        }

        private Rink ParseRink(string rinkName)
        {
            switch (rinkName)
            {
                case "Richmond Ice Centre - Igloo":
                case "RIC Igloo":
                case "Igloo":
                    return Rink.Igloo;

                case "Richmond Ice Centre - Garage":
                case "RIC Garage":
                case "Garage":
                    return Rink.Garage;

                case "Richmond Ice Centre - Pond":
                case "RIC Pond":
                case "Pond":
                    return Rink.Pond;

                case "Richmond Ice Centre - Coliseum":
                case "RIC Coliseum":
                case "Coliseum":
                    return Rink.Coliseum;

                case "Richmond Ice Centre - Forum":
                case "RIC Forum":
                case "Forum":
                    return Rink.Forum;

                case "Richmond Ice Centre - Gardens":
                case "RIC Gardens":
                case "Gardens":
                    return Rink.Gardens;
                
                case "Minoru Arenas - Silver":
                case "Minoru Silver":
                case "Silver":
                    return Rink.Silver;

                case "Minoru Arenas - Stadium":
                case "Minoru Stadium":
                case "Stadium":
                    return Rink.Stadium;

                case "Richmond Olympic Oval - North 2":
                case "Richmond Olympic Oval - North":
                case "Oval North":
                    return Rink.OvalNorth;

                case "Richmond Olympic Oval - South 1":
                case "Richmond Olympic Oval - South":
                case "Oval South":
                    return Rink.OvalSouth;

                default:
                    Console.WriteLine("Unrecognized rink name: '{0}'", rinkName);
                    return Rink.Unknown;
            }
        }
    }
}
