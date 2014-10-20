using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IceScheduler.Parsers;

namespace SchedulerTest
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void ParseTimeRange_BothAM_CorrectTimes()
        {
            // Arrange
            string range = "6:00-7:15am";

            // Act
            TimeSpan[] times = DateTimeParser.ParseTimeRange(range);

            // Assert
            Assert.AreEqual(2, times.Length);
            Assert.AreEqual(TimeSpan.FromHours(6), times[0]);
            Assert.AreEqual(TimeSpan.FromHours(7.25), times[1]);
        }

        [TestMethod]
        public void ParseTimeRange_BothPM_CorrectTimes()
        {
            // Arrange
            string range = "6:00-7:15pm";

            // Act
            TimeSpan[] times = DateTimeParser.ParseTimeRange(range);

            // Assert
            Assert.AreEqual(2, times.Length);
            Assert.AreEqual(TimeSpan.FromHours(18), times[0]);
            Assert.AreEqual(TimeSpan.FromHours(19.25), times[1]);
        }

        [TestMethod]
        public void ParseTimeRange_AMandPM_CorrectTimes()
        {
            // Arrange
            string range = "11:00-12:15pm";

            // Act
            TimeSpan[] times = DateTimeParser.ParseTimeRange(range);

            // Assert
            Assert.AreEqual(2, times.Length);
            Assert.AreEqual(TimeSpan.FromHours(11), times[0]);
            Assert.AreEqual(TimeSpan.FromHours(12.25), times[1]);
        }
    }
}
