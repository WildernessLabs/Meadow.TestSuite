using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Threading.Tasks;

namespace Meadow.TestSuite.Cli
{
    public abstract class CommandBase : ICommand
    {
        public abstract ValueTask ExecuteAsync(IConsole console);

        protected DirectorProvider DirectorProvider { get; }

        public CommandBase(DirectorProvider directorProvider)
        {
            DirectorProvider = directorProvider;
        }

        [CommandOption("transport", 't', Description = "Test command transport", IsRequired = true)]
        public string Transport { get; set; }
    }
}