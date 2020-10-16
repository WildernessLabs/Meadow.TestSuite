using Meadow.Devices;
using Meadow.TestSuite;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestSuite.Unit.Tests")]

namespace MeadowApp
{
    public class Worker : IWorker
    {
        public F7Micro Device { get; set; }
        public ITestRegistry Registry { get; }

        public Worker()
        {
            Registry = new WorkerRegistry();
        }

        public Worker(F7Micro device)
            : this()
        {
            Device = device;
        }
    }
}