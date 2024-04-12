using Meadow;
using Meadow.TestSuite;
using Munit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReleaseValidation.ProjectLab;

public class InProcTestProvider
{
    private Dictionary<string, TestInfo> _cache = new Dictionary<string, TestInfo>();

    public TestInfo[] GetTests()
    {
        return _cache.Values.ToArray();
    }

    public void Load()
    {
        var added = 0;
        var localAssembly = Assembly.GetExecutingAssembly();
        var name = localAssembly.GetName().Name;

        foreach (var t in localAssembly.GetTypes())
        {
            // we have very simple rules here - don't build complex test types
            if (t.IsClass && !t.IsAbstract)
            {
                var ctor = t.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                {
                    continue;
                }
                var methods = t.GetMethods().Where(m =>
                    m.GetCustomAttribute<FactAttribute>() != null
                    && m.GetParameters().Length == 0);

                var count = methods.Count();

                if (count > 0)
                {
                    Resolver.Log.Info($" {t.Name} contains {count} tests.");

                    var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    var device = props.FirstOrDefault(p => p.PropertyType.IsAssignableFrom(typeof(IMeadowDevice)));
                    if (device != null)
                    {
                        Resolver.Log.Info($" {t.Name} has Device property");
                    }

                    foreach (var method in methods)
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
                        if (_cache.ContainsKey(info.ID))
                        {
                            Resolver.Log.Info($" Test {info.ID} already known. Replacing.");
                            _cache[info.ID] = info;
                            added++;
                        }
                        else
                        {
                            _cache.Add(info.ID, info);
                            added++;
                        }
                    }
                }
            }
            else
            {
                Resolver.Log.Info($" Skipping {t.Name}");
            }
        }
    }
}
