using IceScheduler.Slots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Parsers
{
    public class PcahaRinkParser
    {
        public static Rink ParseRink(string rinkName)
        {
            return PcahaRinkMap[rinkName];
        }

        public static Dictionary<string, Rink> PcahaRinkMap = new Dictionary<string, Rink>
        {
            { "Abbotsford Centre Arena", Rink.AbbotsfordCentreArena },
            { "Abbotsford Centre Ice 1 Blue", Rink.AbbotsfordCentreIceBlue },
            { "Abbotsford Recreation Centre", Rink.AbbotsfordARC },
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
            { "Langley Sportsplex 4", Rink.LangleySportsplex4 },
            { "Langley Twin Rinks 1", Rink.LangleyTwin1 },
            { "Langley Twin Rinks 2", Rink.LangleyTwin2 },
            { "MSA Arena - Abbotsford", Rink.AbbotsfordMSA },
            { "Matsqui Recreation Centre", Rink.AbbotsfordMRC },
            { "Minoru Arena Silver Spectrum", Rink.Silver },
            { "Minoru Arena Stadium", Rink.Stadium },
            { "Newton Arena", Rink.Newton },
            { "North Shore Winter Club", Rink.NorthShoreWinterClub },
            { "North Surrey Arena 2", Rink.NorthSurrey2 },
            { "PNE Agrodome", Rink.Agrodome },
            { "Pitt Meadows Arena Volkswagen Blue", Rink.PittMeadowsBlue },
            { "Pitt Meadows Arena Fiat Red", Rink.PittMeadowsRed },
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
            { "South Delta Recreation Centre - Tsawwassen", Rink.SouthDeltaTsawwassen },
            { "Sunset Arena", Rink.Sunset },
            { "Surrey Sport & Leisure Centre Arena 1 Blue", Rink.SurreyLeisure1 },
            { "Surrey Sport & Leisure Centre Arena 2 Purple", Rink.SurreyLeisure2 },
            { "Surrey Sport & Leisure Centre Arena 3 Green", Rink.SurreyLeisure3 },
            { "SS&L 3 Green", Rink.SurreyLeisure3 },
            { "Tilbury Ice", Rink.Tilbury },
            { "Trout Lake Community Centre", Rink.TroutLake },
            { "West Vancouver Ice Arena", Rink.WestVan },
            { "Non-PCAHA Arena", Rink.Unknown },
        };
    }
}
