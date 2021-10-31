using CliFx;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meadow.TestSuite.Cli
{
    class Program
    {
        public static async Task<int> Main()
        {
            var services = new ServiceCollection();
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

        private static void AddCommandsAsServices(IServiceCollection services)
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();

            foreach (var type in assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(CommandBase)) && !t.IsAbstract))
            {
                services.AddTransient(type);
            }
        }


        /*
        private static ITestDirector GetDirector(IOptions options, ICommandSerializer serializer)
        {
            // are we using rest or serial?
            if (!string.IsNullOrEmpty(options.Ethernet))
            {
                if (!IPEndPoint.TryParse(options.Ethernet, out var ep))
                {
                    Console.WriteLine($"Invalid IP End Point '{options.Ethernet}'");
                    return null;
                }
                return new RestTestDirector(ep);
            }
            else if (!string.IsNullOrEmpty(options.Port))
            {
                var transport = new WorkerSerialTransport(serializer, options.Port, options.BaudRate);
                return new SerialTestDirector(serializer, transport);
            }
            else
            {
                Console.WriteLine($"Either a Serial Port or IP End Point is required");
                return null;
            }
        }

        private static int Launch(IOptions options)
        {
            var p = new Program();

            var serializer = new CommandJsonSerializer();
            serializer.UseLibrary = JsonLibrary.SystemTextJson;

            var director = GetDirector(options, serializer);
            if (director == null)
            {
                Console.WriteLine($"No transport to Worker defined");
                return -1;
            }

            Task task = null;

            if (options is UplinkOptions)
            {
                task = p.Uplink(director, options as UplinkOptions);
            }
            else if (options is AssemblyOptions)
            {
                task = p.ProcessAssemblyCommand(director, options as AssemblyOptions);
            }
            else if (options is TestOptions)
            {
                task = p.ProcessTestCommand(director, options as TestOptions);
            }
            else if (options is ResultOptions)
            {
                task = p.ProcessResultCommand(director, options as ResultOptions);
            }

            task?.Wait();

            return 0;
        }

        private async Task Uplink(ITestDirector director, UplinkOptions options)
        {
            Console.WriteLine($"Uplink {options.Source}");

            await director.SendFile(new System.IO.FileInfo( options.Source), options.Destination);
            Console.WriteLine($"  sent.");
        }

        private async Task ProcessAssemblyCommand(ITestDirector director, AssemblyOptions options)
        {
            if (options.List)
            {
                Console.WriteLine($"List of Assemblies:");
                var assemblies = await director.GetAssemblies();

                if (assemblies.Length == 0)
                {
                    Console.WriteLine($"  <none>");
                }
                else
                {
                    foreach (var a in assemblies)
                    {
                        Console.WriteLine($"  {a}");
                    }
                }
            }
        }

        private async Task ProcessTestCommand(ITestDirector director, TestOptions options)
        {
            if (options.List)
            {
                Console.WriteLine($"List of Tests:");

                var result = await director.GetTestNames();

                if ((result == null) || (result.Length == 0))
                {
                    Console.WriteLine($"  <none>");
                }
                else
                {
                    foreach (var t in result)
                    {
                        Console.WriteLine($"  {t}");
                    }
                }
            }
            else if (!string.IsNullOrEmpty(options.Execute))
            {
                // allow a few delimiters
                var names = options.Execute.Split(new char[] { ';', ',', '|' }, StringSplitOptions.RemoveEmptyEntries);

                var results = new List<TestResult>();

                foreach (var name in names)
                {
                    try
                    {
                        results.Add(await director.ExecuteTest(name));
                    }
                    catch (System.IO.FileNotFoundException fe)
                    {
                        Console.WriteLine($"Unable to open the serial port: {fe.Message}");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error running test {name}: {ex.Message}");
                    }
                }

                Console.WriteLine("Executing tests:");

                // TODO: support verbose
                foreach (var r in results)
                {
                    Console.WriteLine($"  {r.TestID} as {r.ResultID}");
                }
            }
        }

        private async Task ProcessResultCommand(ITestDirector director, ResultOptions options)
        {
            TestResult[] results;

            Console.WriteLine($"List of Results:");

            if (!options.ResultID.Equals(Guid.Empty))
            {
                results = new TestResult[] { await director.GetTestResults(options.ResultID) };
            }
            else if (!string.IsNullOrEmpty(options.TestID))
            {
                results = await director.GetTestResults(options.TestID);
            }
            else
            {
                results = await director.GetTestResults();
            }

            if ((results == null) || (results.Length == 0))
            {
                Console.WriteLine($"  <none>");
            }
            else
            {
                // TODO: support verbose
                foreach (var r in results)
                {
                    Console.WriteLine($"  ({r.State})\t{r.TestID}\t{string.Join(',', r.Output)}");
                }
            }
        }
        */
    }
}