using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCloudElevatorWebAPI
{
    public class LogHelper
    {
        public static void Error(string s)
        {
            var fc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(s);
            Console.ForegroundColor = fc;

            Log.Information(s);
        }

        public static void TraceTx(string s)
        {
            var fc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(s);
            Console.ForegroundColor = fc;

            Log.Information(s);
        }

        public static void TraceRx(string s)
        {
            var fc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(s);
            Console.ForegroundColor = fc;

            Log.Information(s);
        }
    }
}
