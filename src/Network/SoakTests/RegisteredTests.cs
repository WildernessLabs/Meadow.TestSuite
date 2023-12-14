using System;
namespace SoakTests;

public static class RegisteredTests
{
    public static ISoakTest[] Tests = new ISoakTest[]
    {
        new HttpGetSoakTest()
    };

    public static ISoakTest GetTest(string testName)
    {
        foreach (var test in Tests)
        {
            if (test.Name == testName)
            {
                return test;
            }
        }

        return null;
    }
}