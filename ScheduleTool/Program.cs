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

            List<IceSlot> slots = new List<IceSlot>();
            for (int i = 0; i < args.Length; i++)
            {
                RavensScheduleParser parser = new RavensScheduleParser();
                slots.AddRange(parser.Parse(args[i]));
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
