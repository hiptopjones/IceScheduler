using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public class AvailableSlot : IceSlot
    {
        public AvailableSlot(IceTime iceTime)
            : base(iceTime)
        {

        }

        public override string ToString()
        {
            return string.Format("Available - {1}", base.ToString());
        }
    }
}
