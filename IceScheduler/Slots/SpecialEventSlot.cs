using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public class SpecialEventSlot : IceSlot
    {
        public string Title { get; private set; }
        public string SubTitle { get; private set; }

        public SpecialEventSlot(IceTime iceTime, string title, string subTitle)
            : base(iceTime)
        {
            Title = title;
            SubTitle = subTitle;
        }

        public override string ToString()
        {
            return string.Format("Special Event - {0} / {1} - {2}", Title, SubTitle, base.ToString());
        }
    }
}
