using Meadow;
using Meadow.Devices;
using Meadow.TestSuite;
using SimpleJson;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        private Worker Worker { get; }

        public MeadowApp()
        {
            Console.WriteLine("+ MeadowApp");

            Worker = new Worker(Device);
            Worker.Start();
            // the above blocks, so we never actually get here

            Thread.Sleep(Timeout.Infinite);

            Console.WriteLine("- MeadowApp");
        }
    }
}