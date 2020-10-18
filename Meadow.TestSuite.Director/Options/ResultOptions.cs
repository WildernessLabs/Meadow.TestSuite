using CommandLine;
using System;

namespace Meadow.TestSuite
{
    [Verb("result", HelpText = "Result Commands")]
    public class ResultOptions : BaseOptions
    {
        [Option('a', "all", Required = false, HelpText = "Get a List of all Results")]
        public bool All { get; set; }
        [Option('r', "result-id", Required = false, HelpText = "ResultID (guid) on which to filter")]
        public Guid ResultID { get; set; }
        [Option('t', "test-id", Required = false, HelpText = "Test ID on which to filter")]
        public string TestID { get; set; }
    }
}
