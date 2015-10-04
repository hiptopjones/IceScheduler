using IceScheduler.Slots;
using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IceScheduler.Parsers
{
    public class InvoiceParser
    {
        // Field indexes
        private const int Facility = 1;
        private const int DayOfWeek = 12;
        private const int StartDate = 17;
        private const int StartTime = 24;
        private const int EndDate = 33;
        private const int EndTime = 41;

        // Parses the input schedule in a CSV format
        public List<IceSlot> Parse(string path)
        {
            List<IceSlot> schedule = new List<IceSlot>();

            bool isPrologue = true;

            foreach (string line in File.ReadAllLines(path))
            {
                if (isPrologue)
                {
                    if (line.Contains("Facility/Equipment"))
                    {
                        isPrologue = false;
                    }

                    continue;
                }

                try
                {
                    IceSlot slot = ParseIntoIceSlot(line);
                    if (slot != null)
                    {
                        schedule.Add(slot);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("End of ice slots detected: [{0}]", line);
                    break;
                }
            }

            return schedule;
        }

        private IceSlot ParseIntoIceSlot(string line)
        {
            // Do not remove empty fields
            string[] fields = line.Split(new[] { ',' }, StringSplitOptions.None);

            // Ignore empty lines
            if (string.IsNullOrEmpty(fields[Facility]))
            {
                return null;
            }

            Rink rink = ParseRink(fields[Facility]);
            DateTime startTime = DateTime.Parse(fields[StartDate] + " " + fields[StartTime]);
            DateTime endTime = DateTime.Parse(fields[EndDate] + " " + fields[EndTime]);

            IceTime iceTime = new IceTime(rink, startTime, endTime);

            AvailableSlot slot = new AvailableSlot(iceTime);
            return slot;
        }

        private Rink ParseRink(string rinkName)
        {
            switch (rinkName)
            {
                case "Richmond Ice Centre - Igloo":
                    return Rink.Igloo;
                case "Richmond Ice Centre - Garage":
                    return Rink.Garage;
                case "Richmond Ice Centre - Pond":
                    return Rink.Pond;
                case "Richmond Ice Centre - Coliseum":
                    return Rink.Coliseum;
                case "Richmond Ice Centre - Forum":
                    return Rink.Forum;
                case "Richmond Ice Centre - Gardens":
                    return Rink.Gardens;
                case "Minoru Arenas - Silver":
                    return Rink.Silver;
                case "Minoru Arenas - Stadium":
                    return Rink.Stadium;
                case "Richmond Olympic Oval - North 2":
                    return Rink.OvalNorth;
                case "Richmond Olympic Oval - South 1":
                    return Rink.OvalSouth;
                default:
                    throw new Exception(string.Format("Unrecognized rink name: '{0}'", rinkName));
            }
        }
    }
}
