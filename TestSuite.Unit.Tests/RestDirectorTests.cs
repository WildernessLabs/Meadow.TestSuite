using System;
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
                new IPEndPoint(IPAddress.Parse("192.168.1.87"),
                8080));
        }

        [Fact]
        public void GetAssembliesTestInvalidAddress()
        {
            Assert.Throws<ArgumentException>(() =>
                {
                    var director = new RestTestDirector("192.168.1.500:8080");
                }
            );
        }

        [Fact]
        public void GetAssembliesTestUnreachableAddress()
        {
            var director = new RestTestDirector(
                new IPEndPoint(IPAddress.Parse("8.8.8.8"),
                8080));

            Assert.Throws<System.AggregateException>(() =>
            {
                director.GetAssemblies().Wait();
            });
        }

        [Fact]
        public async Task GetAssembliesTest()
        {
            var director = GetDirector();
            var assemblies = await director.GetAssemblies();
            // might be zero-length, but should never be null
            Assert.NotNull(assemblies); 
        }

    }
}
