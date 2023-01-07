using CliFx;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using msdi = Microsoft.Extensions.DependencyInjection;

namespace Meadow.TestSuite.Cli
{
    class Program
    {
        public static async Task<int> Main()
        {
            var services = new msdi.ServiceCollection();
            services.AddSingleton<DirectorProvider>();

            AddCommandsAsServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var result = await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .SetExecutableName("mtd")
                .UseTypeActivator(serviceProvider.GetService)
                .Build()
                .RunAsync();

            return result;
        }

        private static void AddCommandsAsServices(msdi.IServiceCollection services)
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();

            foreach (var type in assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(CommandBase)) && !t.IsAbstract))
            {
                services.AddTransient(type);
            }
        }
    }
}