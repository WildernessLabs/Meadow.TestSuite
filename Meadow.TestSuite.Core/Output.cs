using System;

namespace Meadow.TestSuite
{
    public static class Output
    {
        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public static void WriteLineIf(bool condition, string message)
        {
            if (condition)
            {
                Console.WriteLine(message);
            }
        }
    }
}