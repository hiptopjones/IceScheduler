using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public class IceTime
    {
        public Rink Rink { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        // This date is a Sunday well in the past, which enables easier math and detection of relative schedules
        public static readonly DateTime RootSunday = DateTime.Parse("April 1, 1900");

        public IceTime(Rink rink, DateTime start, DateTime end)
        {
            Rink = rink;
            Start = start;
            End = end;
        }

        public IceTime(Rink rink, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            Rink = rink;

            Start = date + startTime;
            End = date + endTime;
        }

        public IceTime(Rink rink, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            Rink = rink;

            DateTime rootDayOfWeek = RootSunday;
            rootDayOfWeek = rootDayOfWeek.AddDays((int)dayOfWeek);

            Start = rootDayOfWeek + startTime;
            End = rootDayOfWeek + endTime;
        }

        public IceTime(Rink rink, DateTime start, TimeSpan duration)
        {
            Rink = rink;
            Start = start;
            End = start + duration;
        }

        public void Adjust(TimeSpan delta)
        {
            Start += delta;
            End += delta;
        }

        public override string ToString()
        {
            // Only include the month and day if it is valid (non-root)
            if (Start - RootSunday < TimeSpan.FromDays(7))
            {
                return string.Format("{0} {1} to {2} {3}", Start.DayOfWeek, Start.ToString("t"), End.ToString("t"), Rink);
            }

            return string.Format("{0} {1} {2} to {3} {4}", Start.DayOfWeek, Start.ToString("M"), Start.ToString("t"), End.ToString("t"), Rink);
        }
    }
}
