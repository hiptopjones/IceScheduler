using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Parsers
{
    public static class DateTimeParser
    {
        // Parses strings like "6:00-7:00am"
        public static TimeSpan[] ParseTimeRange(string range)
        {
            string[] parts = range.Split(new char[] { '-' });

            if (parts.Length != 2)
            {
                throw new ApplicationException(string.Format("unexpected range formatting: {0}", range));
            }

            DateTime startTime = DateTime.Parse(parts[0]);
            DateTime endTime = DateTime.Parse(parts[1]);

            // If they are more than 12 hours apart, then startTime needs to be moved to PM
            if ((endTime - startTime) > TimeSpan.FromHours(12))
            {
                startTime += TimeSpan.FromHours(12);
            }

            return new TimeSpan[] { startTime.TimeOfDay, endTime.TimeOfDay };
        }

    }
}
