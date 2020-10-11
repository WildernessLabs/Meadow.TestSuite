using CommandLine;

namespace Meadow.TestSuite
{
    [Verb("result", HelpText = "Result Commands")]
    public class ResultOptions : BaseOptions
    {
        [Option('l', "list", Required = false, HelpText = "List All Known Test Results")]
        public bool List { get; set; }
    }
}
