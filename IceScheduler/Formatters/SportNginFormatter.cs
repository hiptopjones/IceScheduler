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

                    if (slot is PracticeSlot)
                    {
                        PracticeSlot practiceSlot = (PracticeSlot)slot;

                        title = "Practice";
                        tags = string.Join("|", GetTagsForTeams((TeamBasedIceSlot)slot));
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
                    else
                    {
                        continue;
                    }

                    startDate = slot.IceTime.Start.ToString("M/d/yyyy");
                    startTime = slot.IceTime.Start.ToString("HH:mm");
                    endDate = slot.IceTime.End.ToString("M/d/yyyy");
                    endTime = slot.IceTime.End.ToString("HH:mm");
                    location = slot.IceTime.Rink.ToString();

                    string[] data = new[]
                        {
                            startDate,
                            startTime,
                            endDate,
                            endTime,
                            title,
                            location,
                            tags,
                        };

                    writer.WriteLine(string.Join(",", data));
                }
            }
        }

        private IEnumerable<string> GetTagsForTeams(TeamBasedIceSlot slot)
        {
            return slot.GetParticipatingTeams().Select(GetTag);
        }

        private string GetTag(Team team)
        {
            return team.ToStringNoAssociation().Replace(" ", "");
        }
    }
}
