namespace Meadow.TestSuite
{
    public interface IWorker
    {
        ITestRegistry Registry { get; }
        IResultsStore Results { get; }

        TestResult ExecuteTest(string testID);

        void IndicateState(TestState state);
    }
}