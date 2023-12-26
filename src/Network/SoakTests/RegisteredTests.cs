using System;
namespace SoakTests;

public static class RegisteredTests
{
    /// <summary>
    /// Registered tests.
    /// </summary> 
    public static ISoakTest[] Tests = new ISoakTest[]
    {
        new HttpGetSoakTest(),
        new SocketSoakTest(),
        new SocketAsyncSoakTest(),
    };

    /// <summary>
    /// Look for the test with the given name.
    /// </summary>
    /// <param name="testName">Name of the test to be executed.</param>
    /// <returns>Pointer to the test to be executed or null if the test cannot be found.</returns>
    public static ISoakTest GetTest(string testName)
    {
        foreach (var test in Tests)
        {
            if (test.GetType().Name == testName)
            {
                return test;
            }
        }

        return null;
    }
}