namespace Meadow.TestSuite
{
    public interface IWorker
    {
        ITestRegistry Registry { get; }
        void ExecuteTest(string testID);
    }
}