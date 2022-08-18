using System;
using System.Threading;
using Meadow;

namespace MeadowApp
{
    class Program
    {
        static IApp app;
        public static void Main(string[] args)
        {
            // instantiate and run new meadow app
            app = new MeadowApp();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}