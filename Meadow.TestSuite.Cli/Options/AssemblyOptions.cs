using CommandLine;

namespace Meadow.TestSuite
{
    [Verb("assembly", HelpText = "Assembly commands")]
    public class AssemblyOptions : BaseOptions
    {
        [Option("list", Required = false, HelpText = "List All Known Assemlies")]
        public bool List { get; set; }
//        [Option('c', "clear", Required = false, HelpText = "Delete All Test Assemlies")]
//        public bool Clear { get; set; }
    }
}
