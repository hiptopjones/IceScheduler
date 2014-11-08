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
                rink = ParseRink(fields[Arena]);
            }

            IceTime iceTime = new IceTime(rink, startTime, endTime);

            GameSlot slot = new GameSlot(iceTime, type, homeTeam, awayTeam, otherInfo);
            return slot;
        }

        private static Rink ParseRink(string rinkName)
        {
            return PcahaRinkMap[rinkName];
        }

        public static Dictionary<string, Rink> PcahaRinkMap = new Dictionary<string, Rink>
        {
            { "Abbotsford Centre Ice 1 Blue", Rink.AbbotsfordCentreIceBlue },
            { "Bellingham Sportsplex", Rink.BellinghamSportsplex },
            { "Burnaby Lake Arena", Rink.BurnabyLake },
            { "Burnaby Winter Club", Rink.BurnabyWinterClub },
            { "Chilliwack Twin Rinks 2", Rink.ChilliwackTwin2 },
            { "Cloverdale Arena", Rink.Cloverdale },
            { "Coquitlam Main", Rink.CoquitlamMain },
            { "Coquitlam Recreation", Rink.CoquitlamRec },
            { "George Preston Recreation Centre", Rink.GeorgePreston },
            { "Harry Jerome Rec Centre", Rink.HarryJerome },
            { "Hillcrest Centre Arena", Rink.Hillcrest },
            { "Ice Sports North Shore 1 Red", Rink.IceSportsNorthShoreRed },
            { "Ice Sports North Shore 2 Blue", Rink.IceSportsNorthShoreBlue },
            { "Ice Sports North Shore 3 Green", Rink.IceSportsNorthShoreGreen },
            { "Kensington Arena", Rink.Kensington },
            { "Killarney Community Centre", Rink.Killarney },
            { "Langley Sportsplex 1", Rink.LangleySportsplex1 },
            { "Langley Sportsplex 2", Rink.LangleySportsplex2 },
            { "Langley Twin Rinks 1", Rink.LangleyTwin1 },
            { "MSA Arena - Abbotsford", Rink.AbbotsfordMSA },
            { "Matsqui Recreation Centre", Rink.AbbotsfordMRC },
            { "Minoru Arena Silver Spectrum", Rink.Silver },
            { "Newton Arena", Rink.Newton },
            { "North Shore Winter Club", Rink.NorthShoreWinterClub },
            { "North Surrey Arena 2", Rink.NorthSurrey2 },
            { "PNE Agrodome", Rink.Agrodome },
            { "Pitt Meadows Arena Volkswagen Blue", Rink.PittMeadowsBlue },
            { "Planet Ice Coquitlam 1 Mars", Rink.PlanetIceCoquitlam1 },
            { "Planet Ice Coquitlam 2 Pluto", Rink.PlanetIceCoquitlam2 },
            { "Planet Ice Coquitlam 3 Venus", Rink.PlanetIceCoquitlam3 },
            { "Planet Ice Coquitlam 4 Saturn", Rink.PlanetIceCoquitlam4 },
            { "Planet Ice Delta Canadian", Rink.PlanetIceDeltaCanadian },
            { "Planet Ice Maple Ridge 1 Cam Neely", Rink.PlanetIceMapleRidge1 },
            { "Planet Ice Maple Ridge 2", Rink.PlanetIceMapleRidge2 },
            { "Port Coquitlam Blue", Rink.PocoBlue },
            { "Port Coquitlam Green", Rink.PocoGreen },
            { "Port Moody 2", Rink.PortMoody2 },
            { "Richmond Ice Centre Coliseum", Rink.Coliseum },
            { "Richmond Ice Centre Forum", Rink.Forum },
            { "Richmond Ice Centre Garage", Rink.Garage },
            { "Richmond Ice Centre Igloo", Rink.Igloo },
            { "Richmond Ice Centre Pond", Rink.Pond },
            { "Richmond Olympic Oval North", Rink.OvalNorth },
            { "Richmond Olympic Oval South", Rink.OvalSouth },
            { "Sunset Arena", Rink.Sunset },
            { "Surrey Sport & Leisure Centre Arena 1 Blue", Rink.SurreyLeisure1 },
            { "Surrey Sport & Leisure Centre Arena 2 Purple", Rink.SurreyLeisure2 },
            { "Surrey Sport & Leisure Centre Arena 3 Green", Rink.SurreyLeisure3 },
            { "SS&L 3 Green", Rink.SurreyLeisure3 },
            { "Tilbury Ice", Rink.Tilbury },
            { "West Vancouver Ice Arena", Rink.WestVan }
        };
    }
}
