using Meadow.TestSuite;
using MeadowApp;
using Xunit;

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
