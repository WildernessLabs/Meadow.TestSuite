namespace Meadow.TestSuite
{
    public class GetTestNamesCommand : TestCommand
    {
        public GetTestNamesCommand()
        {
            CommandType = CommandType.EnumerateTests;
        }

        public override void Execute(IWorker worker)
        {
            Result = worker.Registry.GetTestNames();
        }
    }
}