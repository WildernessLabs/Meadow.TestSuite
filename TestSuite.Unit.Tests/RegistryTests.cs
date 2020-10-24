using Meadow.TestSuite;
using MeadowApp;
using System;
using Xunit;
using Xunit.Sdk;

namespace TestSuite.Unit.Tests
{
    public class RegistryTests
    {
        [Fact]
        public void WorkerRegistryTest()
        {
            var r = new WorkerRegistry(null, null);

            var names = r.GetTestsNames();
        }
    }
}
