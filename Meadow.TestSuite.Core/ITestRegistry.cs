namespace Meadow.TestSuite
{
    public interface ITestRegistry
    {
        void RegisterAssembly(string assemblyPath);
        string[] GetAssemblies();
        string[] GetTestNames();
        string[] GetMatchingNames(string testPath);
    }
}