using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using IceScheduler.Slots;
using IceScheduler.Teams;

namespace IceScheduler.Parsers
{
    public class TeamLinkParser
    {
        private enum ColumnIndex
        {
            GameID,
            Group,
            Home,
            Visitor,
            Date,
            Location,
            StartTime,
            EndTime,
            Status,
            Remarks,
        }

        private const string TeamLinkScheduleUrl = "http://teamlink.ca/scheduleReport.htm";

        private List<KeyValuePair<string,string>> GetFormValues(string tierName, GameType gameType)
        {
            Dictionary<string, string[]> leagueGames = new Dictionary<string,string[]> {
                {"Tyke", new string[] { "gameseason", "14", "gamedivision", "9", "gamecategory", "22", "gamecompetition", "3", "gametier", "21"}},
                {"Novice", new string[] { "gameseason", "14", "gamedivision", "8", "gamecategory", "22", "gamecompetition", "3", "gametier", "24"}},
                {"Atom C1", new string[] { "gameseason", "14", "gamedivision", "7", "gamecategory", "22", "gamecompetition", "3", "gametier", "24"}},
                {"Atom C2", new string[] { "gameseason", "14", "gamedivision", "7", "gamecategory", "22", "gamecompetition", "3", "gametier", "21"}},
                {"Atom C3", new string[] { "gameseason", "14", "gamedivision", "7", "gamecategory", "22", "gamecompetition", "3", "gametier", "19"}},
                {"Peewee A", new string[] { "gameseason", "14", "gamedivision", "6", "gamecategory", "22", "gamecompetition", "3", "gametier", "87"}},
                {"Peewee C1", new string[] { "gameseason", "14", "gamedivision", "6", "gamecategory", "22", "gamecompetition", "3", "gametier", "19"}},
                {"Bantam A", new string[] { "gameseason", "14", "gamedivision", "5", "gamecategory", "22", "gamecompetition", "3", "gametier", "87"}},
                {"Bantam C1", new string[] { "gameseason", "14", "gamedivision", "5", "gamecategory", "22", "gamecompetition", "3", "gametier", "24"}},
                {"Bantam C2", new string[] { "gameseason", "14", "gamedivision", "5", "gamecategory", "22", "gamecompetition", "3", "gametier", "19"}},
                {"Midget A", new string[] { "gameseason", "14", "gamedivision", "4", "gamecategory", "22", "gamecompetition", "3", "gametier", "87"}},
                {"Midget C1", new string[] { "gameseason", "14", "gamedivision", "4", "gamecategory", "22", "gamecompetition", "3", "gametier", "24"}},
                {"Midget C2", new string[] { "gameseason", "14", "gamedivision", "4", "gamecategory", "22", "gamecompetition", "3", "gametier", "19"}},
                {"Juvenile", new string[] { "gameseason", "14", "gamedivision", "3", "gamecategory", "22", "gamecompetition", "3", "gametier", "24"}},
            };

            return ToKeyValuePairList(leagueGames[tierName]);
        }

        private List<KeyValuePair<string, string>> ToKeyValuePairList(string[] flatPairs)
        {
            List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < flatPairs.Length; i += 2)
            {
                pairs.Add(new KeyValuePair<string, string>(flatPairs[i], flatPairs[i + 1]));
            }

            return pairs;
        }

        public List<IceSlot> ParseSchedule()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = SubmitFormValues(web, GetFormValues("Peewee A", GameType.League), TeamLinkScheduleUrl);

            return ParseSchedule(doc);
        }

        // http://dbanck.de/2010/01/09/post-data-with-net-html-agility-pack/
        private HtmlDocument SubmitFormValues(HtmlWeb web, List<KeyValuePair<string, string>> pairs, string url)
        {
            // Attach a temporary delegate to handle attaching
            // the post back data
            HtmlAgilityPack.HtmlWeb.PreRequestHandler handler = delegate(HttpWebRequest request)
            {
                // Turn key/value pairs into a POST body
                string payload = string.Join("&", pairs.Select((pair => string.Format("{0}={1}", pair.Key, pair.Value))));
                Console.WriteLine("payload: {0}", payload);
                byte[] bytes = Encoding.ASCII.GetBytes(payload.ToCharArray());

                request.ContentLength = bytes.Length;
                request.ContentType = "application/x-www-form-urlencoded";

                Stream reqStream = request.GetRequestStream();
                reqStream.Write(bytes, 0, bytes.Length);

                return true;
            };

            web.PreRequest += handler;
            HtmlDocument doc = web.Load (url, "POST");
            web.PreRequest -= handler;

            return doc;
        }

        private List<IceSlot> ParseSchedule(HtmlDocument doc)
        {
            List<IceSlot> slots = new List<IceSlot>();

            HtmlNode table = doc.GetElementbyId("reportdetailtable");
            HtmlNodeCollection rows = table.SelectNodes("./tr");

            foreach (var row in rows)
            {
                // Iterate all columns in this row
                HtmlNodeCollection cols = row.SelectNodes("./td");
                if (cols == null)
                {
                    // Ignore header (no <td>, just <th>)
                    continue;
                }

                string gameId = cols[(int)ColumnIndex.GameID].InnerText.Trim();
                string groupName = cols[(int)ColumnIndex.Group].InnerText.Trim();
                Team homeTeam = PcahaTeamParser.ParseIntoTeam(cols[(int)ColumnIndex.Home].InnerText.Trim());
                Team awayTeam = PcahaTeamParser.ParseIntoTeam(cols[(int)ColumnIndex.Visitor].InnerText.Trim());
                DateTime date = DateTime.Parse(cols[(int)ColumnIndex.Date].InnerText.Trim());
                Rink rink = PcahaRinkParser.ParseRink(cols[(int)ColumnIndex.Location].InnerText.Trim());
                TimeSpan startTime = TimeSpan.Parse(cols[(int)ColumnIndex.StartTime].InnerText.Trim());
                TimeSpan endTime = TimeSpan.Parse(cols[(int)ColumnIndex.EndTime].InnerText.Trim());

                if (rink == Rink.Unknown)
                {
                    date = DateTime.MinValue;
                    startTime = TimeSpan.FromDays(0);
                    endTime = TimeSpan.FromDays(0);
                }

                IceTime iceTime = new IceTime(rink, date, startTime, endTime);

                slots.Add(new GameSlot(iceTime, GameType.League, homeTeam, awayTeam, gameId));
            }

            return slots;
        }

        private Team ParseTeam(string teamName)
        {
            return null;
        }

        private Rink ParseRink(string rinkName)
        {
            return Rink.Unknown;
        }
    }
}