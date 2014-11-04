using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IceScheduler.Parsers;
using IceScheduler.Teams;
using System.Collections.Generic;

namespace SchedulerTest
{
    [TestClass]
    public class ParsingUtilitiesTest
    {
        [TestMethod]
        public void ParseTimeRange_BothAM_CorrectTimes()
        {
            // Arrange
            string range = "6:00-7:15am";

            // Act
            TimeSpan[] times = ParsingUtilities.ParseTimeRange(range);

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
            TimeSpan[] times = ParsingUtilities.ParseTimeRange(range);

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
            TimeSpan[] times = ParsingUtilities.ParseTimeRange(range);

            // Assert
            Assert.AreEqual(2, times.Length);
            Assert.AreEqual(TimeSpan.FromHours(11), times[0]);
            Assert.AreEqual(TimeSpan.FromHours(12.25), times[1]);
        }

        [TestMethod]
        public void ParseRavensTeams_OneTeam_CorrectTeams()
        {
            // Arrange
            string name = "Bantam C2";
            List<Team> expectedTeams = new List<Team>
            {
                new Team(Association.RichmondGirls, Division.Bantam, Level.C, 2),
            };

            // Act
            List<Team> actualTeams = ParsingUtilities.ParseRavensTeams(name);

            // Assert
            CollectionAssert.AreEqual(expectedTeams, actualTeams);
        }

        [TestMethod]
        public void ParseRavensTeams_TwoTeams_CorrectTeams()
        {
            // Arrange
            string name = "AC1/BC2";
            List<Team> expectedTeams = new List<Team>
            {
                new Team(Association.RichmondGirls, Division.Atom, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Bantam, Level.C, 2),
            };

            // Act
            List<Team> actualTeams = ParsingUtilities.ParseRavensTeams(name);

            // Assert
            CollectionAssert.AreEqual(expectedTeams, actualTeams);
        }

        [TestMethod]
        public void ParseRavensTeams_ThreeTeams_CorrectTeams()
        {
            // Arrange
            string name = "AC1/PA2/BC2";
            List<Team> expectedTeams = new List<Team>
            {
                new Team(Association.RichmondGirls, Division.Atom, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Peewee, Level.A, 2),
                new Team(Association.RichmondGirls, Division.Bantam, Level.C, 2),
            };

            // Act
            List<Team> actualTeams = ParsingUtilities.ParseRavensTeams(name);

            // Assert
            CollectionAssert.AreEqual(expectedTeams, actualTeams);
        }
    }
}
