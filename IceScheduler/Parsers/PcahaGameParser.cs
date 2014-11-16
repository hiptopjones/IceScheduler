using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Parsers
{
    public static class PcahaGameParser
    {
        // Field indexes
        private const int GameNumber = 0;
        private const int DayOfWeek = 1;
        private const int Date = 2;
        private const int StartTime = 3;
        private const int EndTime = 4;
        private const int HomeTeam = 5;
        private const int AwayTeam = 6;
        private const int Arena = 7;

        public static GameSlot ParseIntoGameSlot(GameType type, string line)
        {
            // Do not remove empty fields
            string[] fields = line.Split(new[] { ',' }, StringSplitOptions.None);

            string otherInfo = fields[GameNumber];

            Team homeTeam = PcahaTeamParser.ParseIntoTeam(fields[HomeTeam]);
            Team awayTeam = PcahaTeamParser.ParseIntoTeam(fields[AwayTeam]);

            Rink rink = Rink.Unknown;
            DateTime startTime = DateTime.MinValue;
            DateTime endTime = DateTime.MinValue;

            // Not a conflict game?
            if (!string.IsNullOrEmpty(fields[Date]))
            {
                startTime = DateTime.Parse(fields[Date] + " " + fields[StartTime]);
                endTime = DateTime.Parse(fields[Date] + " " + fields[EndTime]);
                rink = PcahaRinkParser.ParseRink(fields[Arena]);
            }

            IceTime iceTime = new IceTime(rink, startTime, endTime);

            GameSlot slot = new GameSlot(iceTime, type, homeTeam, awayTeam, otherInfo);
            return slot;
        }
    }
}
