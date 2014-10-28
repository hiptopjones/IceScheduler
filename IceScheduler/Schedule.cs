using IceScheduler.Slots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler
{
    // NOTES:
    // This breaks down if the schedule represents a period of time where there is no ice
    // For example, the schedule might be Monday -> Sunday, but there's no ice on Monday.
    // This class would report Tuesday -> Sunday. Another place it breaks down is if the 
    // end of the last ice time wrapped over to Monday. This class would still report
    // this as the schedule for Monday -> Sunday.
    public class Schedule
    {
        public List<IceSlot> Slots { get; private set; }

        public DateTime Start
        {
            get
            {
                return Slots.First().IceTime.Start.Date;
            }
        }

        public DateTime End
        {
            get
            {
                return Slots.Last().IceTime.Start.Date;
            }
        }

        public Schedule(List<IceSlot> slots)
        {
            // TODO: Need to copy to a read-only collection, so that this cannot be modified after the fact.
            Slots = slots.OrderBy(s => s.IceTime.Start).ToList();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var slot in Slots.OrderBy(s => s.IceTime.Start))
            {
                builder.AppendLine(slot.ToString());
            }

            return builder.ToString();
        }
    }
}
