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
        "MQTTnet",
        "Meadow.ProjectLab",
        "Meadow.TestSuite",
    };

    private const string RepoRoot = "/home/pi/repos";
    //private const string RepoRoot = "f:\\temp\\git_test";

    private const string HcomSerialPort = "/dev/ttyACM0";
    //private const string HcomSerialPort = "COM5";

    private static async Task Main(string[] args)
    {
        Console.WriteLine("TestSuite Test Coordinator");

        var agent = new MeadowStackBuildAgent(CoreRepositoryStack, new DirectoryInfo(RepoRoot));

        //        var c = new WebhookSmeeProxy();

        //agent.CloneTree();
        Console.WriteLine("Pulling Develop...");
        var success = agent.PullTree("develop");
        Console.WriteLine("Building the Core stack...");
        success = agent.Build("Meadow.Core/source/Meadow.Core/implementations/f7/Meadow.F7/Meadow.F7.csproj");

        Console.WriteLine("Building the Project Lab test app...");

        //var director = new HcomTestDirector("F:/temp/git_test/Meadow.TestSuite/src", "hcom:COM5");
        var testAssemblyPath = Path.Combine(RepoRoot, "Meadow.TestSuite/src/TestSuite/Test Assemblies");
        var director = new HcomTestDirector(testAssemblyPath, $"hcom:{HcomSerialPort}");

        var result = await director.ExecuteTests("ReleaseValidation.ProjectLab");

        Console.WriteLine("Done");
    }
}
