using IceScheduler;
using IceScheduler.Formatters;
using IceScheduler.Parsers;
using IceScheduler.Slots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Arguments: '{0}'", string.Join("','", args));

            if (args.Length < 1)
            {
                PrintUsage();
                return;
            }

            List<IceSlot> slots = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-r")
                {
                    i++;

                    RavensScheduleParser parser = new RavensScheduleParser();
                    slots = parser.Parse(args[i]);
                }
                else if (args[i] == "-p")
                {
                    i++;

                    PcahaScheduleParser parser = new PcahaScheduleParser();
                    slots = parser.Parse(GameType.Tiering, args[i]);
                }

                if (args[i] == "-s")
                {
                    i++;
                    
                    switch (args[i])
                    {
                        case "Time":
                            slots = slots.OrderBy(s => s.IceTime.Start).ToList();
                            break;
                    }
                }

                if (args[i] == "-w")
                {
                    i++;

                    RavensScheduleFormatter formatter = new RavensScheduleFormatter();
                    formatter.WriteSchedule(slots, args[i]);
                }
            }

            Console.WriteLine("Number of slots: {0}", slots.Count);

            foreach (IceSlot slot in slots)
            {
                Console.WriteLine(slot);
            }


            WaitForKey();
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    Don't be a dumbass!");

            WaitForKey();
        }

        static void WaitForKey()
        {
            Console.Write("Hit any key to continue...");
            Console.ReadKey();
        }
    }
}
