using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Threading.Tasks;

namespace Meadow.TestSuite.Cli
{
    [Command("assembly list")]
    public class AssemblyListCommand : CommandBase
    {
        public AssemblyListCommand(DirectorProvider directorProvider)
            : base(directorProvider)
        {
        }

        public override async ValueTask ExecuteAsync(IConsole console)
        {
            if (Transport == null)
            {
                console.Output.WriteLine($"A transport is required");
                return;
            }

            var director = DirectorProvider.GetDirector(Transport);

            console.Output.WriteLine($"List of Assemblies:");
            var assemblies = await director.GetAssemblies();

            if (assemblies.Length == 0)
            {
                console.Output.WriteLine($"  <none>");
            }
            else
            {
                foreach (var a in assemblies)
                {
                    console.Output.WriteLine($"  {a}");
                }
            }
        }
    }
}