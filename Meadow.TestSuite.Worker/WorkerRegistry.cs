using Meadow.Hardware;
using Munit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Meadow.TestSuite
{
    internal interface ITestProvider : ITestRegistry
    {
        TestInfo GetTest(string id);
    }

    internal class TestInfo
    {
        public string AssemblyName { get; set; }
        public string TypeName { get; set; }
        public string TestName { get; set; }
        public MethodInfo TestMethod { get; set; }
        public ConstructorInfo TestConstructor { get; set; }
        public PropertyInfo DeviceProperty { get; set; }

        public string ID
        {
            get => $"{AssemblyName}.{TypeName}.{TestName}";
        }
    }

    // TODO: coalesce with Director registry type and pull out an interface?
    internal class WorkerRegistry : ITestProvider
    {
        // tests are [assembly].[class].[method]
        // they are divined by looking for public methods with a [Fact] attribute and nothing else

        private Dictionary<string, TestInfo> m_cache = new Dictionary<string, TestInfo>();

        public WorkerRegistry()
        {

        }

        public WorkerRegistry(string testDirectory)
        {
            RegisterAssembliesInFolder(testDirectory);
        }

        public TestInfo GetTest(string id)
        {
            lock (m_cache)
            {
                if (m_cache.ContainsKey(id))
                {
                    return m_cache[id];
                }
                return null;
            }
        }

        private void RegisterAssembliesInFolder(string assemblyPath)
        {
            var di = new DirectoryInfo(assemblyPath);
            if (!di.Exists) return;

            foreach(var file in di.GetFiles("*.dll"))
            {
                try
                {
                    RegisterAssembly(file.FullName);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Unable to register '{file.Name}': {ex.Message}");
                }
            }
        }

        public void RegisterAssembly(string assemblyPath)
        {
            Console.WriteLine($"Getting info for '{assemblyPath}'...");
            var f = new FileInfo(assemblyPath);
            if (!f.Exists)
            {
                Console.WriteLine("FileInfo says target does not exist.");

                Console.WriteLine($"Checking with File for '{assemblyPath}'...");
                if (!File.Exists(assemblyPath))
                {
                    Console.WriteLine("File.Exists says target does not exist.");
                    return;
                }
            }

            Console.WriteLine($"Loading assembly '{assemblyPath}'...");
            var asm = Assembly.LoadFrom(assemblyPath);
            var added = 0;

            Console.WriteLine("Looking for tests...");
            foreach(var t in asm.GetTypes())
            {
                // we have very simple rules here - don't build complex test types
                if(t.IsClass && !t.IsAbstract)
                {
                    Console.WriteLine($" Checking {t.Name}...");
                    var ctor = t.GetConstructor(Type.EmptyTypes);
                    if(ctor == null)
                    {
                        Console.WriteLine($" {t.Name} has no public parameterless constructor. Skipping.");
                    }
                    else
                    {
                        var methods = t.GetMethods().Where(m => 
                            m.GetCustomAttribute<FactAttribute>() != null
                            && m.GetParameters().Length == 0);

                        var count = methods.Count();

                        Console.WriteLine($" {t.Name} contains {count} tests.");

                        if (count > 0)
                        {
                            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                            foreach(var p in props)
                            {
                                Console.WriteLine($" prop {p.Name}: {p.PropertyType.Name}");
                            }
                            var device = props.FirstOrDefault(p => p.PropertyType == typeof(IIODevice));
                            Console.WriteLine($" device = {device}");

                            var name = asm.GetName().Name;

                            foreach(var method in methods)
                            {
                                var info = new TestInfo
                                {
                                    AssemblyName = name,
                                    TypeName = t.Name,
                                    TestName = method.Name,
                                    TestMethod = method,
                                    TestConstructor = ctor,
                                    DeviceProperty = device
                                };
                                if(m_cache.ContainsKey(info.ID))
                                {
                                    Console.WriteLine($" Test {info.ID} already known. Replacing.");
                                    m_cache[info.ID] = info;
                                    added++;
                                }
                                else
                                {
                                    m_cache.Add(info.ID, info);
                                    added++;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($" Skipping {t.Name}");
                }
            }

            if (added == 0)
            {
                Console.WriteLine("No tests found.");
                return;
            }
            else
            {
                Console.WriteLine($"{added} {(added == 1 ? "test" : "tests")} found.");
            }
        }

        public string[] GetAssemblies()
        {
            lock (m_cache)
            {
                return m_cache
                    .Values
                    .Select(v => v.AssemblyName)
                    .Distinct()
                    .ToArray();
            }
        }

        public string[] GetTestsNames()
        {
            lock (m_cache)
            {
                return m_cache
                    .Values
                    .Select(v => v.ID)
                    .ToArray();
            }
        }
    }
}