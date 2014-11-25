using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IceScheduler.Slots;
using IceScheduler.Teams;

namespace IceScheduler.Formatters
{
    // Writes out a format that can be uploaded into TeamPages
    // Produces one file per team.
    public class TeamPagesFormatter
    {
        public void WriteAllSchedules(List<IceSlot> slots, string outputDirectory)
        {
            // Ensure the output directory eixsts
            Directory.CreateDirectory(outputDirectory);

            foreach (Team team in GetRavensTeams())
            {
                Console.WriteLine("Writing slots for {0}...", team);

                List<IceSlot> slotsForTeam = GetSlotsForTeam(slots, team);

                string teamName = team.ToStringNoAssociation().Replace(" ", "");
                WriteSchedule(slotsForTeam, CreateTeamSchedulePath(outputDirectory, teamName));

                Console.WriteLine();
            }

            Console.WriteLine("Writing available slots...");
            List<IceSlot> availableSlots = slots.Where(s => (s is AvailableSlot)).ToList();
            WriteSchedule(availableSlots, CreateTeamSchedulePath(outputDirectory, "Available"));
        }

        private string CreateTeamSchedulePath(string outputDirectory, string teamName)
        {
            return Path.Combine(outputDirectory, string.Format("{0}_schedule.csv", teamName));
        }

        private void WriteSchedule(List<IceSlot> slots, string schedulePath)
        {
            using (StreamWriter writer = new StreamWriter(schedulePath))
            {
                string[] fields = new[]
                    {
                        "Type",
                        "Date",
                        "Start",
                        "Finish",
                        "Opponent",
                        "Home",
                        "Location",
                    };

                writer.WriteLine(string.Join(",", fields));

                foreach (IceSlot slot in slots)
                {
                    Console.WriteLine(slot.ToString());

                    string type = string.Empty;
                    string date = string.Empty;
                    string start = string.Empty;
                    string finish = string.Empty;
                    string opponent = string.Empty;
                    string home = string.Empty;
                    string location = string.Empty;

                    if (slot is PracticeSlot)
                    {
                        type = "Practice";
                        opponent = "Ignore";
                    }
                    else if (slot is TeamSkillDevelopmentSlot)
                    {
                        TeamSkillDevelopmentSlot skillsSlot = slot as TeamSkillDevelopmentSlot;
                        if (skillsSlot.Name != "Development")
                        {
                            // Ignore Accelerator, etc.
                            continue;
                        }

                        type = "Skills Development";
                        opponent = "Skills Session";
                    }
                    else if (slot is GameSlot)
                    {
                        GameSlot gameSlot = slot as GameSlot;

                        type = "League Game";
                        home = "@";
                        opponent = gameSlot.HomeTeam.ToStringVersus();

                        // TODO: Need to handle case where two Ravens teams play each other
                        if (gameSlot.HomeTeam.Association == Association.RichmondGirls)
                        {
                            home = "vs.";
                            opponent = gameSlot.AwayTeam.ToStringVersus();
                        }
                    }
                    else if (slot is AvailableSlot)
                    {
                        type = "Available";
                    }

                    date = slot.IceTime.Start.ToString("dd-MMM-yyyy");
                    start = slot.IceTime.Start.ToString("hh:mm tt");
                    finish = slot.IceTime.End.ToString("hh:mm tt");
                    location = RinkMapping[slot.IceTime.Rink];

                    string[] data = new[]
                        {
                            type,
                            date,
                            start,
                            finish,
                            opponent,
                            home,
                            location,
                        };

                    writer.WriteLine(string.Join(",", data));
                }
            }
        }

        private List<Team> GetRavensTeams()
        {
            return new List<Team>
            {
                new Team(Association.RichmondGirls, Division.Tyke, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Tyke, Level.C, 2),
                new Team(Association.RichmondGirls, Division.Novice, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Novice, Level.C, 2),
                new Team(Association.RichmondGirls, Division.Atom, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Atom, Level.C, 2),
                new Team(Association.RichmondGirls, Division.Atom, Level.C, 3),
                new Team(Association.RichmondGirls, Division.Peewee, Level.A, 1),
                new Team(Association.RichmondGirls, Division.Peewee, Level.A, 2),
                new Team(Association.RichmondGirls, Division.Peewee, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Bantam, Level.A, 1),
                new Team(Association.RichmondGirls, Division.Bantam, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Bantam, Level.C, 2),
                new Team(Association.RichmondGirls, Division.Midget, Level.A, 1),
                new Team(Association.RichmondGirls, Division.Midget, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Midget, Level.C, 2),
                new Team(Association.RichmondGirls, Division.Juvenile, Level.C, 1),
            };
        }

        private List<IceSlot> GetSlotsForTeam(List<IceSlot> slots, Team team)
        {
            List<IceSlot> slotsForTeam = new List<IceSlot>();

            foreach (IceSlot slot in slots)
            {
                if (slot is PracticeSlot)
                {
                    PracticeSlot practiceSlot = slot as PracticeSlot;
                    if (practiceSlot.Teams.Contains(team))
                    {
                        slotsForTeam.Add(practiceSlot);
                    }
                }
                else if (slot is TeamSkillDevelopmentSlot)
                {
                    TeamSkillDevelopmentSlot skillsSlot = slot as TeamSkillDevelopmentSlot;
                    if (skillsSlot.Teams.Contains(team))
                    {
                        slotsForTeam.Add(skillsSlot);
                    }
                }
                else if (slot is GameSlot)
                {
                    GameSlot gameSlot = slot as GameSlot;
                    if (gameSlot.AwayTeam.Equals(team) || gameSlot.HomeTeam.Equals(team))
                    {
                        slotsForTeam.Add(gameSlot);
                    }
                }
                else if (slot is TournamentSlot)
                {
                    // Ignore
                }
            }

            return slotsForTeam;
        }

        private static Dictionary<Rink, string> RinkMapping = new Dictionary<Rink, string>() 
        {
            { Rink.BuckNEar, "Buck 'n' Ear" },  
            { Rink.AbbotsfordCentreIceBlue, "Abbotsford Centre Ice - 1 (Blue)" },  
            { Rink.AbbotsfordMRC, "Abbotsford MRC" },  
            { Rink.AbbotsfordMSA, "Abbotsford MSA" },  
            { Rink.BellinghamSportsplex, "Bellingham Sportsplex" },  
            { Rink.BurnabyLake, "Burnaby Lake Arena" },  
            { Rink.Kensington, "Burnaby Kensington Arena" },  
            { Rink.ChilliwackTwin2, "Chilliwack Twin Rinks - #2" },  
            { Rink.Cloverdale, "Cloverdale Arena" },  
            { Rink.CoquitlamMain, "Coquitlam 1 Main" },  
            { Rink.CoquitlamRec, "Coquitlam 2 Poirier Sport and Rec. Centre" },  
            { Rink.GeorgePreston, "George Preston Recreation Centre (Langley Civic Centre)" },  
            { Rink.PlanetIceDeltaCanadian, "Great Pacific Forum - Canadian" },  
            { Rink.HarryJerome, "Harry Jerome Recreation Centre" },  
            { Rink.Hillcrest, "Hillcrest Arena / Vancouver Olympic Centre (Riley Park)" },  
            { Rink.IceSportsNorthShoreRed, "Ice Sports North Van - #1 Red" },  
            { Rink.IceSportsNorthShoreBlue, "Ice Sports North Van - #2 Blue" },  
            { Rink.IceSportsNorthShoreGreen, "Ice Sports North Van - #3 Green" },  
            { Rink.Killarney, "Killarney Arena" },  
            { Rink.LangleySportsplex1, "Langley Sportsplex - 1 Green" },  
            { Rink.LangleySportsplex2, "Langley Sportsplex - 2 Blue" },  
            { Rink.LangleyTwin1, "Langley Twin" },  
            { Rink.Silver, "Minoru - Silver" },  
            { Rink.Stadium, "Minoru - Stadium" },  
            { Rink.Newton, "Newton Community Centre" },  
            { Rink.NorthShoreWinterClub, "North Shore Winter club" },  
            { Rink.NorthSurrey2, "North Surrey - #2" },  
            { Rink.Agrodome, "Pacific National Exhibition - Agrodome" }, 
            { Rink.PittMeadowsBlue, "Pitt Meadows Arenas - Volkswagen Blue" }, 
            { Rink.PlanetIceCoquitlam1, "Planet Ice Coquitlam - Mars-1" }, 
            { Rink.PlanetIceCoquitlam2, "Planet Ice Coquitlam - Pluto-2" }, 
            { Rink.PlanetIceCoquitlam3, "Planet Ice Coquitlam - Venus-3" }, 
            { Rink.PlanetIceCoquitlam4, "Planet Ice Coquitlam - Saturn-4" }, 
            { Rink.PlanetIceMapleRidge1, "Planet Ice Ridge Meadows - Cam Neely #1" }, 
            { Rink.PlanetIceMapleRidge2, "Planet Ice Ridge Meadows - Ice #2" }, 
            { Rink.PocoBlue, "Port Coquitlam - Blue (new)" }, 
            { Rink.PocoGreen, "Port Coquitlam - Green (old)" },
            { Rink.PortMoody2, "Port Moody - 2 (new)" }, 
            { Rink.Coliseum, "Richmond Ice Centre - Coliseum" }, 
            { Rink.Forum, "Richmond Ice Centre - Forum" }, 
            { Rink.Garage, "Richmond Ice Centre - Garage" }, 
            { Rink.Gardens, "Richmond Ice Centre - Garden" }, 
            { Rink.Igloo, "Richmond Ice Centre - Igloo" }, 
            { Rink.Pond, "Richmond Ice Centre - Pond" }, 
            { Rink.OvalNorth, "Richmond Oympic Oval - North" }, 
            { Rink.OvalSouth, "Richmond Oympic Oval - South" }, 
            { Rink.Sunset, "Sunset Arena" }, 
            { Rink.SurreyLeisure1, "Surrey Sports & Leisure Centre (Fleetwood) - #1" }, 
            { Rink.SurreyLeisure2, "Surrey Sports & Leisure Centre (Fleetwood) - #2" }, 
            { Rink.SurreyLeisure3, "Surrey Sports & Leisure Centre (Fleetwood) - #3" }, 
            { Rink.Tilbury, "Tilbury Ice" }, 
            { Rink.WestVan, "West Vancouver Rec Centre" }
        };
    }
}
