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

            string inputType = null;
            string inputPath = null;
            string outputType = null;
            string outputPath = null;
            string sortType = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-r")
                {
                    inputType = args[++i];
                }
                else if (args[i] == "-w")
                {
                    outputType = args[++i];
                }
                else if (args[i] == "-i")
                {
                    inputPath = args[++i];
                }
                else if (args[i] == "-o")
                {
                    outputPath = args[++i];
                }
                else if (args[i] == "-s")
                {
                    sortType = args[++i];
                }
                else
                {
                    Console.WriteLine("Unrecognized switch: {0}", args[i]);
                    PrintUsage();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(inputType))
            {
                if (string.IsNullOrEmpty(inputPath))
                {
                    Console.WriteLine("Input path cannot be empty if specifying an input type.");
                    PrintUsage();
                    return;
                }

                if (inputType.ToLower() == "ravens")
                {
                    RavensScheduleParser parser = new RavensScheduleParser();
                    slots = parser.Parse(inputPath);
                }
                else if (inputType.ToLower() == "pcaha")
                {
                    // TODO: How to get game type in here?
                    PcahaScheduleParser parser = new PcahaScheduleParser();
                    slots = parser.Parse(GameType.Tiering, inputPath);
                }
                else if (inputType.ToLower() == "flat")
                {
                    FlatListParser parser = new FlatListParser();
                    slots = parser.Parse(inputPath);
                }
                else
                {
                    Console.WriteLine("Unrecognized input type: {0}", inputType);
                    PrintUsage();
                    return;
                }
            }

            Console.WriteLine("Processing {0} slots.", slots.Count);

            if (!string.IsNullOrEmpty(sortType))
            {
                if (sortType.ToLower() == "time")
                {
                    slots = slots.OrderBy(s => s.IceTime.Start).ToList();
                }
                else
                {
                    Console.WriteLine("Unrecognized sort type: {0}", sortType);
                    PrintUsage();
                    return;
                }
            }
            
            if (!string.IsNullOrEmpty(outputType))
            {
                if (string.IsNullOrEmpty(outputPath))
                {
                    Console.WriteLine("Output path cannot be empty if specifying an output type.");
                    PrintUsage();
                    return;
                }

                if (outputType.ToLower() == "flat")
                {
                    FlatListFormatter formatter = new FlatListFormatter();
                    formatter.WriteSchedule(slots, outputPath);
                }
                else if (outputType.ToLower() == "ravens")
                {
                    RavensScheduleFormatter formatter = new RavensScheduleFormatter();
                    formatter.WriteSchedule(slots, outputPath);
                }
                else
                {
                    Console.WriteLine("Unrecognized output type: {0}", outputType);
                    PrintUsage();
                    return;
                }
            }

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
