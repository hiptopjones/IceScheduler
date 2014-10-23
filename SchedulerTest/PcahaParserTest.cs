using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IceScheduler.Parsers;
using IceScheduler.Teams;
using System.Collections.Generic;
using IceScheduler.Slots;

namespace SchedulerTest
{
    [TestClass]
    public class PcahaParserTest
    {
        [TestMethod]
        public void ParseGame_ScheduledGame_ReturnsExpectedGameSlot()
        {
            // Arrange
            string gameLine = "AQ9001,Thursday,10/09/2014,18:15,19:30,North Shore Female Atom C1,Richmond Girls Atom C1,Harry Jerome Rec Centre";

            // Act
            GameSlot slot = PcahaGameParser.ParseIntoGameSlot(GameType.League, gameLine);

            // Assert
            Assert.AreEqual(GameType.League, slot.Type);
            Assert.AreEqual(Rink.HarryJerome, slot.IceTime.Rink);
            Assert.AreEqual(Association.RichmondGirls, slot.AwayTeam.Association);
            Assert.AreEqual(Association.NorthShoreFemale, slot.HomeTeam.Association);
            Assert.AreEqual(DateTime.Parse("10/09/2014 6:15pm"), slot.IceTime.Start);
            Assert.AreEqual(DateTime.Parse("10/09/2014 7:30pm"), slot.IceTime.End);
        }

        [TestMethod]
        public void ParseGame_ConflictGame_ReturnsExpectedGameSlot()
        {
            // Arrange
            string gameLine = "AQ9001,,,,,North Shore Female Atom C1,Richmond Girls Atom C1,";

            // Act
            GameSlot slot = PcahaGameParser.ParseIntoGameSlot(GameType.League, gameLine);

            // Assert
            Assert.AreEqual(GameType.League, slot.Type);
            Assert.AreEqual(Rink.Unknown, slot.IceTime.Rink);
            Assert.AreEqual(Association.RichmondGirls, slot.AwayTeam.Association);
            Assert.AreEqual(Association.NorthShoreFemale, slot.HomeTeam.Association);
            Assert.AreEqual(DateTime.MinValue, slot.IceTime.Start);
            Assert.AreEqual(DateTime.MinValue, slot.IceTime.End);
        }

        [TestMethod]
        public void ParseTeam_ShortAssociationName_ReturnsExpectedTeam()
        {
            // Arrange
            string teamName = "Richmond Girls Midget C1";

            // Act
            Team team = PcahaTeamParser.ParseIntoTeam(teamName);

            // Assert
            Assert.AreEqual(team.Association, Association.RichmondGirls);
            Assert.AreEqual(team.Division, Division.Midget);
            Assert.AreEqual(team.Level, Level.C);
            Assert.AreEqual(team.Flight, 1);
        }

        [TestMethod]
        public void ParseTeam_LongAssociationName_ReturnsExpectedTeam()
        {
            // Arrange
            string teamName = "North Shore Winter Club Female Atom A5";

            // Act
            Team team = PcahaTeamParser.ParseIntoTeam(teamName);

            // Assert
            Assert.AreEqual(team.Association, Association.NorthShoreWinterClubFemale);
            Assert.AreEqual(team.Division, Division.Atom);
            Assert.AreEqual(team.Level, Level.A);
            Assert.AreEqual(team.Flight, 5);
        }

        [ExpectedException(typeof(KeyNotFoundException))]
        [TestMethod]
        public void ParseTeam_UnknownAssociation_ThrowsException()
        {
            // Arrange
            string teamName = "Some Unknown Assocation Atom A5";

            // Act
            Team team = PcahaTeamParser.ParseIntoTeam(teamName);

            // Assert
            // Expect exception
        }

        [ExpectedException(typeof(KeyNotFoundException))]
        [TestMethod]
        public void ParseTeam_UnknownDivision_ThrowsException()
        {
            // Arrange
            string teamName = "Richmond Girls Foobar A5";

            // Act
            Team team = PcahaTeamParser.ParseIntoTeam(teamName);

            // Assert
            // Expect exception
        }

        [ExpectedException(typeof(KeyNotFoundException))]
        [TestMethod]
        public void ParseTeam_UnknownLevel_ThrowsException()
        {
            // Arrange
            string teamName = "Richmond Girls Bantam R5";

            // Act
            Team team = PcahaTeamParser.ParseIntoTeam(teamName);

            // Assert
            // Expect exception
        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void ParseTeam_MissingFlight_ThrowsException()
        {
            // Arrange
            string teamName = "Richmond Girls Bantam R";

            // Act
            Team team = PcahaTeamParser.ParseIntoTeam(teamName);

            // Assert
            // Expect exception
        }
    }
}
