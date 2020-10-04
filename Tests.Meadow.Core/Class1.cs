using System;
using Meadow.TestsSuite;
using Munit;

namespace MeadowLibary
{
    public class LEDTest : IExecutableTest
    {
        public Func<TestResult> TestFunction { get => ExecuteTest; }
        public string Name { get => "Sample test"; }

        private TestResult ExecuteTest()
        {
            Console.WriteLine("ExecuteTest...");

            return new TestResult
            {
                CompletionDate = DateTime.Now,
                State = TestState.Success
            };
        }

        [Fact]
        public void FactMethod()
        {

        }
    }
}