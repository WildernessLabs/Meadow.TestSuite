namespace Meadow.TestSuite
{
    public enum CommandType : long
    {
        UplinkFile,
        EnumerateAssemblies,
        EnumerateTests,
        ExecuteTests,
        GetTestResults,
        DeleteAssemblies,
        GetDutInfo
    }
}