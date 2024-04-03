using Meadow.TestSuite;

namespace TestSuite.Coordinator;

public interface ITestRunner
{
}

internal class ConsoleTestAppRunner
{
    public ConsoleTestAppRunner(string appPath)
    {
    }

}

internal class Program
{
    private static readonly string[] CoreRepositoryStack =
    {
        "Meadow.Units",
        "Meadow.Contracts",
        "Meadow.Core",
        "Meadow.Foundation",
        "Meadow.Logging",
        "Meadow.Modbus",
        "Meadow.ProjectLab",
        "MQTTNet",
        "Meadow.ProjectLab",
        "Meadow.TestSuite",
    };

    private static async Task Main(string[] args)
    {
        Console.WriteLine("TestSuite Test Coordinator");

        var agent = new MeadowStackBuildAgent(CoreRepositoryStack, new DirectoryInfo(@"f:\temp\git_test"));
        /*
        //        var c = new WebhookSmeeProxy();

        //agent.CloneTree();
        Console.WriteLine("Pulling Develop...");
        var success = agent.PullTree("develop");
        Console.WriteLine("Building the Core stack...");
        success = agent.Build("Meadow.Core/source/Meadow.Core.sln");

        Console.WriteLine("Pulling the Project Lab test app...");
        */

        Console.WriteLine("Building the Project Lab test app...");

        //var director = new HcomTestDirector("F:/temp/git_test/Meadow.TestSuite/src", "hcom:COM5");
        var director = new HcomTestDirector("F:/repos/wilderness/Meadow.TestSuite/src", "hcom:COM5");
        var names = await director.GetTestNames();

        var result = await director.ExecuteTest("ProjectLabBaseTests");

        //        await director.BuildTest(TestTarget.MeadowF7, "Meadow.TestSuite/src/ProjectLabBaseTests/ProjectLabBaseTests.csproj");

        Console.WriteLine("Done");

        var l = Console.ReadLine();
    }
}
