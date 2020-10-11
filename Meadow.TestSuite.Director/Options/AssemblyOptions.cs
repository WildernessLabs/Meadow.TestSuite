using CommandLine;

namespace Meadow.TestSuite
{
    [Verb("assembly", HelpText = "Assembly commands")]
    public class AssemblyOptions : BaseOptions
    {
        [Option('l', "list", Required = false, HelpText = "List All Known Assemlies")]
        public bool List { get; set; }
    }
}
