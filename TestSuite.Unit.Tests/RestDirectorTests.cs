using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
                new IPEndPoint(IPAddress.Parse("192.168.0.15"),
                8080));
        }

        private FileInfo GetTestAssemblySource()
        {
            return new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests.Meadow.Core.dll"));
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

        [Fact]
        public async Task FileManagementTest()
        {
            var director = GetDirector();

            var sourceFile = GetTestAssemblySource();

            // create a destiantion name that is unique (in case the test assembly is already there)
            var remoteName = $"{Path.GetFileNameWithoutExtension(sourceFile.Name)}.{DateTime.UtcNow.Ticks}.dll";

            await director.SendFile(sourceFile, remoteName);
            var updatedAssemblies = await director.GetAssemblies();

            Assert.Contains(remoteName, updatedAssemblies);

        }

        [Fact]
        public async Task GetTestNamesTest()
        {
            var director = GetDirector();
            var names = await director.GetTestNames();
            // might be zero-length, but should never be null
            Assert.NotNull(names);
        }

        [Fact]
        public async Task ExecuteTestTest()
        {
            var director = GetDirector();
            var results = await director.ExecuteTest("Tests.Meadow.Core.LEDTests.LedTestFunction");
            // might be zero-length, but should never be null
            Assert.NotNull(results);
        }

        [Fact]
        public async Task GetAllResultsTest()
        {
            var director = GetDirector();
            var results = await director.GetTestResults();
            // might be zero-length, but should never be null
            Assert.NotNull(results);
        }
    }
}
