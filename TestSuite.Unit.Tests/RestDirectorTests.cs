using System.Net;
using System.Threading.Tasks;
using Meadow.TestSuite;
using Xunit;

namespace TestSuite.Unit.Tests
{
    public class RestDirectorTests
    {
        private RestTestDirector GetDirector()
        {
            return new RestTestDirector(
                new IPEndPoint(IPAddress.Parse("192.168.1.88"),
                8080));
        }

        [Fact]
        public async Task GetAssembliesTest()
        {
            var director = GetDirector();
            var assemblies = await director.GetAssemblies();
        }
    }
}
