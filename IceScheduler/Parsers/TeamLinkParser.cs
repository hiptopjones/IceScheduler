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

        public List<IceSlot> ParseSchedule()
        {
            List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("gameseason", "14"));
            pairs.Add(new KeyValuePair<string, string>("gamedivision", "6"));
            pairs.Add(new KeyValuePair<string, string>("gamecategory", "22"));
            pairs.Add(new KeyValuePair<string, string>("gamecompetition", "3"));
            pairs.Add(new KeyValuePair<string, string>("gametier", "87"));

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = SubmitFormValues(web, pairs, TeamLinkScheduleUrl);

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

        /*
<tr>
<td align="center">ML9503</td>
<td align="center">Red Group</td>
<td>
<span>Vancouver Girls Midget C2</span>
</td>
<td>
<span>Langley Girls Midget C2</span>
</td>
<td align="center">Thu Nov 13, 2014</td>
<td>PNE Agrodome</td>
<td align="center">19:45</td>
<td align="center">21:00</td>
<td>Played</td>
<td>&nbsp;</td>
</tr>
         */
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