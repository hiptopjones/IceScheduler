using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public abstract class IceSlot
    {
        public IceTime IceTime { get; private set; }

        public IceSlot(IceTime iceTime)
        {
            IceTime = iceTime;  
        }

        public override string ToString()
        {
            return IceTime.ToString();
        }
    }
}
