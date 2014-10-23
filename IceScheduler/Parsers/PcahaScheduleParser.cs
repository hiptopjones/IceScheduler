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
    public class PcahaScheduleParser
    {
        // Parses the input schedule in a CSV format
        public List<IceSlot> Parse(GameType gameType, string path)
        {
            List<IceSlot> schedule = new List<IceSlot>();

            foreach (string line in File.ReadAllLines(path))
            {
                // Ignore headers
                // TODO: Should parse out the field names, to remove file layout dependency
                if (line.StartsWith("Game #"))
                {
                    Console.WriteLine("Skipping header line: [{0}]", line);
                    continue;
                }

                IceSlot slot = PcahaGameParser.ParseIntoGameSlot(gameType, line);
                if (slot != null)
                {
                    schedule.Add(slot);
                }
            }

            return schedule;
        }
    }
}
