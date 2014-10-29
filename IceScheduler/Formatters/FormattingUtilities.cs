using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Formatters
{
    class FormattingUtilities
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
            return string.Format("{0}-{1}", iceTime.Start.ToString("h:mm"), iceTime.End.ToString("h:mmtt").ToLower());
        }
    }
}
