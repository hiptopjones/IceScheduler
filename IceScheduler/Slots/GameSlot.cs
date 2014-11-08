using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public class GameSlot : IceSlot
    {
        public GameType Type { get; private set; }
        public Team HomeTeam { get; private set; }
        public Team AwayTeam { get; private set; }
        public string OtherInfo { get; private set; }

        public GameSlot(IceTime iceTime, GameType type, Team homeTeam, Team awayTeam)
            : this(iceTime, type, homeTeam, awayTeam, string.Empty)
        {

        }

        public GameSlot(IceTime iceTime, GameType type, Team homeTeam, Team awayTeam, string otherInfo)
            : base(iceTime)
        {
            Type = type;
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            OtherInfo = otherInfo;
        }

        public override string ToString()
        {
            string otherInfo = string.IsNullOrEmpty(OtherInfo) ? string.Empty : string.Format(" ({0})", OtherInfo);
            return string.Format("{0} Game - {1} vs. {2} - {3}{4}", Type, HomeTeam, AwayTeam, base.ToString(), otherInfo);
        }
    }
}
