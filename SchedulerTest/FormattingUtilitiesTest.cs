using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IceScheduler.Teams;
using System.Collections.Generic;
using IceScheduler.Formatters;
using IceScheduler.Slots;

namespace SchedulerTest
{
    [TestClass]
    public class FormattingUtilitiesTest
    {
        [TestMethod]
        public void GetTimeRange_TimeSpanMorning_ReturnsExpected()
        {
            // Arrange
            TimeSpan start = TimeSpan.FromHours(6);
            TimeSpan end = TimeSpan.FromHours(7.5);

            // Act
            string timeRange = FormattingUtilities.GetTimeRange(start, end);

            // Assert
            Assert.AreEqual("6:00-7:30am", timeRange);
        }

        [TestMethod]
        public void GetTimeRange_TimeSpanAcrossNoon_ReturnsExpected()
        {
            // Arrange
            TimeSpan start = TimeSpan.FromHours(11);
            TimeSpan end = TimeSpan.FromHours(12.5);

            // Act
            string timeRange = FormattingUtilities.GetTimeRange(start, end);

            // Assert
            Assert.AreEqual("11:00-12:30pm", timeRange);
        }

        [TestMethod]
        public void GetTimeRange_TimeSpanAfternoon_ReturnsExpected()
        {
            // Arrange
            TimeSpan start = TimeSpan.FromHours(16);
            TimeSpan end = TimeSpan.FromHours(17.5);

            // Act
            string timeRange = FormattingUtilities.GetTimeRange(start, end);

            // Assert
            Assert.AreEqual("4:00-5:30pm", timeRange);
        }


        [TestMethod]
        public void GetTimeRange_DateTime_ReturnsExpected()
        {
            // Arrange
            DateTime start = new DateTime(2014, 10, 12, 6, 0, 0);
            DateTime end = new DateTime(2014, 10, 12, 7, 30, 0);

            // Act
            string timeRange = FormattingUtilities.GetTimeRange(start, end);

            // Assert
            Assert.AreEqual("6:00-7:30am", timeRange);
        }

        [TestMethod]
        public void GetTimeRange_IceTime_ReturnsExpected()
        {
            // Arrange
            DateTime start = new DateTime(2014, 10, 12, 6, 0, 0);
            DateTime end = new DateTime(2014, 10, 12, 7, 30, 0);
            IceTime iceTime = new IceTime(Rink.Stadium, start, end);

            // Act
            string timeRange = FormattingUtilities.GetTimeRange(iceTime);

            // Assert
            Assert.AreEqual("6:00-7:30am", timeRange);
        }

        [TestMethod]
        public void GetCompositeTeamName_OneTeam_ReturnsExepcted()
        {
            // Arrange
            List<Team> teams = new List<Team>
            {
                new Team(Association.RichmondGirls, Division.Peewee, Level.A, 1),
            };

            // Act
            string compositeName = FormattingUtilities.GetCompositeTeamName(teams);

            // Assert
            Assert.AreEqual("Peewee A1", compositeName);
        }

        [TestMethod]
        public void GetCompositeTeamName_TwoTeams_ReturnsExepcted()
        {
            // Arrange
            List<Team> teams = new List<Team>
            {
                new Team(Association.RichmondGirls, Division.Bantam, Level.C, 2),
                new Team(Association.RichmondGirls, Division.Peewee, Level.A, 1),
            };

            // Act
            string compositeName = FormattingUtilities.GetCompositeTeamName(teams);

            // Assert
            Assert.AreEqual("BC2/PA1", compositeName);
        }
    }
}
