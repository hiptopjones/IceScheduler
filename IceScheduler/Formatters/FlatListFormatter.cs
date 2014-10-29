using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Formatters
{
    public class FlatListFormatter
    {
        public void WriteSchedule(List<IceSlot> slots, string path)
        {
            StringBuilder builder = new StringBuilder();

            // DayOfWeek, Date, StartTime, EndTime, Rink, Type, Type-Specific-Fields
            foreach (IceSlot slot in slots)
            {
                List<string> fields = new List<string>();

                IceTime iceTime = slot.IceTime;
                fields.Add(iceTime.Start.DayOfWeek.ToString());
                fields.Add(iceTime.Start.ToString("d"));
                fields.Add(iceTime.Start.ToString("t"));
                fields.Add(iceTime.End.ToString("t"));
                fields.Add(iceTime.Rink.ToString());

                if (slot is AvailableSlot)
                {
                    fields.Add("Available");
                }
                else if (slot is PracticeSlot)
                {
                    fields.Add("Practice");
                    
                    PracticeSlot practiceSlot = slot as PracticeSlot;
                    fields.Add(FormattingUtilities.GetCompositeTeamName(practiceSlot.Teams));
                }
                else if (slot is TeamSkillDevelopmentSlot)
                {
                    fields.Add("TeamSkills");
                    
                    TeamSkillDevelopmentSlot skillsSlot = slot as TeamSkillDevelopmentSlot;
                    fields.Add(FormattingUtilities.GetCompositeTeamName(skillsSlot.Teams));
                }
                else if (slot is OtherSkillDevelopmentSlot)
                {
                    fields.Add("OtherSkills");

                    OtherSkillDevelopmentSlot skillsSlot = slot as OtherSkillDevelopmentSlot;
                    fields.Add(skillsSlot.Name);
                }
                else if (slot is GameSlot)
                {
                    fields.Add("Game");

                    GameSlot gameSlot = slot as GameSlot;
                    fields.Add(gameSlot.Type.ToString());
                    fields.Add(gameSlot.HomeTeam.ToStringNoAssociation());
                    fields.Add(gameSlot.AwayTeam.ToString());
                }
                else
                {
                    throw new Exception(string.Format("Unhandled slot type: {0}", slot));
                }


                builder.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(path, builder.ToString());
        }
    }
}
