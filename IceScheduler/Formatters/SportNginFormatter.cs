using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IceScheduler.Slots;
using IceScheduler.Teams;
using IceScheduler.Utilities;

namespace IceScheduler.Formatters
{
    // Writes out a format that can be uploaded into SportNgin
    public class SportNginFormatter
    {
        public void WriteSchedule(List<IceSlot> slots, string schedulePath)
        {
            using (StreamWriter writer = new StreamWriter(schedulePath))
            {
                string[] fields = new[]
                    {
                        "Start_Date",
                        "Start_Time",
                        "End_Date",
                        "End_Time",
                        "Title",
                        "Location",
                        "Tags",
                        "Event_Type",
                        "Team1_ID",
                        "Team2_ID",
                        "Team2_Name",
                        "Team1_Is_Home",
                        "Game_ID",
                    };

                writer.WriteLine(string.Join(",", fields));

                foreach (IceSlot slot in slots)
                {
                    if (slot is TournamentSlot)
                    {
                        // Ignore tournament slots
                        continue;
                    }

                    Console.WriteLine(slot.ToString());

                    string startDate = string.Empty;
                    string startTime = string.Empty;
                    string endDate = string.Empty;
                    string endTime = string.Empty;
                    string title = string.Empty;
                    string location = string.Empty;
                    string tags = string.Empty;
                    string eventType = string.Empty;
                    string team1Id = string.Empty;
                    string team2Id = string.Empty;
                    string team2Name = string.Empty;
                    string team1IsHome = "0";
                    string gameId = string.Empty;

                    if (slot is PracticeSlot)
                    {
                        PracticeSlot practiceSlot = (PracticeSlot)slot;

                        title = "Practice";
                    }
                    else if (slot is TeamSkillDevelopmentSlot)
                    {
                        TeamSkillDevelopmentSlot skillsSlot = slot as TeamSkillDevelopmentSlot;
                        if (skillsSlot.Name == "Evaluations")
                        {
                            Team team = skillsSlot.Teams[0];
                            title = string.Format("{0} {1} Evaluations", team.Division.ToString(), team.Level.ToString());
                        }
                        else if (skillsSlot.Name == "Development")
                        {
                            title = "Skills Development";
                        }
                        else
                        {
                            // Ignore Accelerator, etc.
                            continue;
                        }

                        tags = string.Join("|", GetTagsForTeams((TeamBasedIceSlot)slot));
                    }
                    else if (slot is AvailableSlot)
                    {
                        title = "Available Ice";
                        tags = "Available";
                    }
                    else if (slot is GameSlot)
                    {
                        GameSlot gameSlot = (GameSlot)slot;

                        eventType = "Game";
                        gameId = gameSlot.OtherInfo;

                        if (gameSlot.HomeTeam.Association == Association.RichmondGirls)
                        {
                            title = string.Format("{0} Game vs. {1}", gameSlot.Type, gameSlot.AwayTeam.ToStringVersus());

                            team1Id = GetTag(gameSlot.HomeTeam);
                            if (gameSlot.AwayTeam.Association == Association.RichmondGirls)
                            {
                                team2Id = GetTag(gameSlot.AwayTeam);
                            }
                            else
                            {
                                team2Name = gameSlot.AwayTeam.ToStringVersus();
                            }

                            team1IsHome = "1";
                        }
                        else if (gameSlot.AwayTeam.Association == Association.RichmondGirls)
                        {
                            title = string.Format("{0} Game @ {1}", gameSlot.Type, gameSlot.HomeTeam.ToStringVersus());

                            team1Id = GetTag(gameSlot.AwayTeam);
                            if (gameSlot.HomeTeam.Association == Association.RichmondGirls)
                            {
                                team2Id = GetTag(gameSlot.HomeTeam);
                            }
                            else
                            {
                                team2Name = gameSlot.HomeTeam.ToStringVersus();
                            }

                            team1IsHome = "0";
                        }
                        else
                        {
                            Console.WriteLine("Non-Richmond game slot: {0}", slot);
                            continue;
                        }

                        if (gameSlot.IceTime.Rink == Rink.Unknown)
                        {
                            Console.WriteLine("Conflict/Unscheduled game: {0}", slot);
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unexpected ice slot: {0}", slot);
                        continue;
                    }

                    if (slot is TeamBasedIceSlot)
                    {
                        tags = string.Join("|", GetTagsForTeams((TeamBasedIceSlot)slot));
                    }

                    startDate = slot.IceTime.Start.ToString("M/d/yyyy");
                    startTime = slot.IceTime.Start.ToString("HH:mm");
                    endDate = slot.IceTime.End.ToString("M/d/yyyy");
                    endTime = slot.IceTime.End.ToString("HH:mm");
                    location = RinkMapping[slot.IceTime.Rink];

                    string[] data = new[]
                        {
                            startDate,
                            startTime,
                            endDate,
                            endTime,
                            title,
                            location,
                            tags,
                            eventType,
                            team1Id,
                            team2Id,
                            team2Name,
                            team1IsHome,
                            gameId,
                        };

                    writer.WriteLine(string.Join(",", data));
                }
            }
        }

        private IEnumerable<string> GetTagsForTeams(TeamBasedIceSlot slot)
        {
            return slot.GetParticipatingTeams().Select(GetTag).Where(t => !string.IsNullOrEmpty(t));
        }

        private string GetTag(Team team)
        {
            // Non-Richmond teams do not have tags on our site
            if (team.Association != Association.RichmondGirls)
            {
                return null;
            }

            return team.ToStringNoAssociation().Replace(" ", "");
        }

        private static Dictionary<Rink, string> RinkMapping = new Dictionary<Rink, string>() 
        {
            { Rink.BuckNEar, "Buck 'n' Ear" },  
            { Rink.AbbotsfordCentreArena, "Abbotsford Centre Arena" },  
            { Rink.AbbotsfordCentreIceBlue, "Abbotsford Centre Ice - 1 (Blue)" },  
            { Rink.AbbotsfordMRC, "Abbotsford MRC" },  
            { Rink.AbbotsfordMSA, "Abbotsford MSA" },  
            { Rink.AbbotsfordARC, "Abbotsford Recreation Centre" },  
            { Rink.BellinghamSportsplex, "Bellingham Sportsplex" },  
            { Rink.BurnabyLake, "Burnaby Lake Arena" },  
            { Rink.BurnabyWinterClub, "Burnaby Winter Club" },  
            { Rink.Kensington, "Burnaby Kensington Arena" },  
            { Rink.ChilliwackTwin2, "Chilliwack Twin Rinks - #2" },  
            { Rink.Cloverdale, "Cloverdale Arena" },  
            { Rink.CoquitlamMain, "Coquitlam 1 Main" },  
            { Rink.CoquitlamRec, "Coquitlam 2 Poirier Sport and Rec. Centre" },  
            { Rink.ExcellentIceBlue, "Excellent Ice 2 Blue" },  
            { Rink.GeorgePreston, "George Preston Recreation Centre (Langley Civic Centre)" },  
            { Rink.PlanetIceDeltaCanadian, "Great Pacific Forum - Canadian" },  
            { Rink.HarryJerome, "Harry Jerome Recreation Centre" },  
            { Rink.Hillcrest, "Hillcrest Arena / Vancouver Olympic Centre (Riley Park)" },  
            { Rink.IceSportsNorthShoreRed, "Ice Sports North Van - #1 Red" },  
            { Rink.IceSportsNorthShoreBlue, "Ice Sports North Van - #2 Blue" },  
            { Rink.IceSportsNorthShoreGreen, "Ice Sports North Van - #3 Green" },  
            { Rink.Killarney, "Killarney Arena" },  
            { Rink.LangleySportsplex1, "Langley Sportsplex - #1 Green" },  
            { Rink.LangleySportsplex2, "Langley Sportsplex - #2 Blue" },  
            { Rink.LangleyTwin1, "Langley Twin - #1" }, 
            { Rink.LangleyTwin2, "Langley Twin - #2" },  
            { Rink.Silver, "Minoru Silver" },  
            { Rink.Stadium, "Minoru Stadium" },  
            { Rink.Newton, "Newton Community Centre" },  
            { Rink.NorthShoreWinterClub, "North Shore Winter Club" },  
            { Rink.NorthSurrey2, "North Surrey - #2" },  
            { Rink.Agrodome, "Pacific National Exhibition - Agrodome" }, 
            { Rink.PittMeadowsBlue, "Pitt Meadows Arenas - Volkswagen Blue" }, 
            { Rink.PittMeadowsRed, "Pitt Meadows Arenas - Fiat Red" }, 
            { Rink.PlanetIceCoquitlam1, "Planet Ice Coquitlam - Mars-1" }, 
            { Rink.PlanetIceCoquitlam2, "Planet Ice Coquitlam - Pluto-2" }, 
            { Rink.PlanetIceCoquitlam3, "Planet Ice Coquitlam - Venus-3" }, 
            { Rink.PlanetIceCoquitlam4, "Planet Ice Coquitlam - Saturn-4" }, 
            { Rink.PlanetIceMapleRidge1, "Planet Ice Ridge Meadows - Cam Neely #1" }, 
            { Rink.PlanetIceMapleRidge2, "Planet Ice Ridge Meadows - Ice #2" }, 
            { Rink.PocoBlue, "Port Coquitlam - Blue (new)" }, 
            { Rink.PocoGreen, "Port Coquitlam - Green (old)" },
            { Rink.PortMoody2, "Port Moody - 2 (new)" }, 
            { Rink.Coliseum, "RIC Coliseum" }, 
            { Rink.Forum, "RIC Forum" }, 
            { Rink.Garage, "RIC Garage" }, 
            { Rink.Gardens, "RIC Gardens" }, 
            { Rink.Igloo, "RIC Igloo" }, 
            { Rink.Pond, "RIC Pond" }, 
            { Rink.OvalNorth, "Oval North" }, 
            { Rink.OvalSouth, "Oval South" }, 
            { Rink.SouthDeltaTsawwassen, "South Delta Recreation Centre - Tsawwassen" }, 
            { Rink.Sunset, "Sunset Arena" }, 
            { Rink.SurreyLeisure1, "Surrey Sports & Leisure Centre (Fleetwood) - #1" }, 
            { Rink.SurreyLeisure2, "Surrey Sports & Leisure Centre (Fleetwood) - #2" }, 
            { Rink.SurreyLeisure3, "Surrey Sports & Leisure Centre (Fleetwood) - #3" }, 
            { Rink.Tilbury, "Tilbury Ice" }, 
            { Rink.TroutLake, "Trout Lake Community Centre" }, 
            { Rink.WestVan, "West Vancouver Recreation Centre" }
        };
    }
}
