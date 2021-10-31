using CommandLine;

namespace Meadow.TestSuite
{
    [Verb("test", HelpText = "Test Commands")]
    public class TestOptions : BaseOptions
    {
        [Option('l', "list", Required = false, HelpText = "List All Known Tests")]
        public bool List { get; set; }
        [Option("run", Required = false, HelpText = "Execute the specified test")]
        public string Execute { get; set; }
    }
}
