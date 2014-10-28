﻿using System;
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

        public IceTime(Rink rink, DateTime start, DateTime end)
        {
            Rink = rink;
            Start = start;
            End = end;
        }

        public IceTime(Rink rink, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            Rink = rink;

            // This date is a Sunday well in the past, which enables easier math and detection of relative schedules
            DateTime rootDayOfWeek = DateTime.Parse("April 1, 1900");
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

        public override string ToString()
        {
            return string.Format("{0} {1} to {2} {3}", Start.DayOfWeek, Start.ToString("t"), End.ToString("t"), Rink);
        }
    }
}
