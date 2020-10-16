using System;
using System.IO;
using System.Linq;

namespace Meadow.TestSuite
{
    public class GetAssembliesCommand : TestCommand
    {
        public GetAssembliesCommand()
        {
            CommandType = CommandType.EnumerateAssemblies;
        }

        public override void Execute(IWorker worker)
        {
            Result = worker.Registry.GetAssemblies();
        }
    }
}