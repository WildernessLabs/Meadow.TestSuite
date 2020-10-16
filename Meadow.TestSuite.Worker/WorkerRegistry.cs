﻿using Munit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Meadow.TestSuite
{
    internal class TestInfo
    {
        public string AssemblyName { get; set; }
        public string TypeName { get; set; }
        public string TestName { get; set; }
        public MethodInfo TestMethod { get; set; }
        public string ID
        {
            get => $"{AssemblyName}.{TypeName}.{TestName}";
        }
    }

    // TODO: coalesce with Director registry type and pull out an interface?
    internal class WorkerRegistry : ITestRegistry
    {
        // tests are [assembly].[class].[method]
        // they are divined by looking for public methods with a [Fact] attribute and nothing else

        private Dictionary<string, TestInfo> m_cache = new Dictionary<string, TestInfo>(); 

        public WorkerRegistry()
        {
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
                            var name = asm.GetName().Name;

                            foreach(var method in methods)
                            {
                                var info = new TestInfo
                                {
                                    AssemblyName = name,
                                    TypeName = t.Name,
                                    TestName = method.Name,
                                    TestMethod = method
                                };
                                if(m_cache.ContainsKey(info.ID))
                                {
                                    Console.WriteLine($" Test {info.ID} already known. Ignoring.");
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