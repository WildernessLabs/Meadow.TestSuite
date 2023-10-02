namespace Meadow.TestSuite
{
    public class GetWorkerInfoCommand : TestCommand
    {
        public GetWorkerInfoCommand()
        {
            CommandType = CommandType.GetDutInfo;
        }

        public override void Execute(IWorker worker)
        {
            Result = worker.Registry.GetAssemblies();
        }
    }

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