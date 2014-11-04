using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Formatters
{
    public class FormattingUtilities
    {
        public static string GetCompositeTeamName(List<Team> teams)
        {
            if (teams.Count > 1)
            {
                return string.Join("/", teams.Select(t => string.Format("{0}{1}{2}", t.Division.ToString().Substring(0, 1), t.Level, t.Flight)));
            }

            return teams.First().ToStringNoAssociation();
        }

        public static string GetTimeRange(IceTime iceTime)
        {
            return GetTimeRange(iceTime.Start, iceTime.End);
        }

        public static string GetTimeRange(TimeSpan start, TimeSpan end)
        {
            DateTime today = DateTime.Today;
            return GetTimeRange(today + start, today + end);
        }

        public static string GetTimeRange(DateTime start, DateTime end)
        {

            return string.Format("{0}-{1}", start.ToString("h:mm"), end.ToString("h:mmtt").ToLower());
        }
    }
}
