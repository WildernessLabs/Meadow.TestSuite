namespace Meadow.TestSuite
{
    public interface ITestRegistry
    {
        void RegisterAssembly(string assemblyPath);
        void Clear();
        string[] GetAssemblies();
        string[] GetTestNames();
        string[] GetMatchingNames(string testPath);
    }
}