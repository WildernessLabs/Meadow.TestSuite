using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("App")]

namespace Munit
{
    public static class Assert
    {
        internal static bool ShowDebug { get; set; } = false;

        private static string CreateFailedMessage(string baseMessage, string userMessage)
        {
            return $"{baseMessage}{(userMessage == null ? string.Empty : $": {userMessage}")}";
        }

        /// <summary>
        /// Forces a test failure
        /// </summary>
        /// <param name="userMessage">The message to show</param>
        public static void Fail(string userMessage)
        {
            var msg = CreateFailedMessage("Assert.Fail()", userMessage);

            throw new TestFailedException(msg);
        }

        /// <summary>
        /// Verifies the condition is true
        /// </summary>
        /// <param name="condition">The condition to be tested</param>
        /// <param name="userMessage">The message to show when the condition is not true</param>
        public static void True(bool condition, string userMessage = null)
        {
            if (!condition)
            {
                var msg = CreateFailedMessage("Assert.True() Failure", userMessage);

                throw new TestFailedException(msg);
            }
        }

        /// <summary>
        /// Verifies the condition is false
        /// </summary>
        /// <param name="condition">The condition to be tested</param>
        /// <param name="userMessage">The message to show when the condition is not false</param>
        public static void False(bool condition, string userMessage = null)
        {
            if (condition)
            {
                var msg = CreateFailedMessage("Assert.False() Failure", userMessage);

                throw new TestFailedException(msg);
            }
        }

        /// <summary>
        /// Verifies the test object is not null
        /// </summary>
        /// <param name="o">The object to be tested</param>
        /// <param name="userMessage">The message to show when the test object is null</param>
        public static void NotNull(object o, string userMessage = null)
        {
            if (o == null)
            {
                var msg = CreateFailedMessage("Assert.NotNull() Failure", userMessage);

                throw new TestFailedException(msg);
            }
        }

        /// <summary>
        /// Verifies the test object is null
        /// </summary>
        /// <param name="o">The object to be tested</param>
        /// <param name="userMessage">The message to show when the test object is not null</param>
        public static void Null(object o, string userMessage = null)
        {
            if (o != null)
            {
                var msg = CreateFailedMessage("Assert.Null() Failure", userMessage);

                throw new TestFailedException(msg);
            }
        }

        /// <summary>
        /// Verifies two objects are equal
        /// </summary>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expected">The expected value to test against</param>
        /// <param name="userMessage">The message to show when the objects are not equal</param>
        public static void Equal(object expected, object actual, string userMessage = null)
        {
            if (!actual.Equals(expected))
            {
                var msg = CreateFailedMessage($"Assert.Equal() Failure. Expected {expected}, Actual {actual}", userMessage);

                throw new TestFailedException(msg);
            }
        }

        /// <summary>
        /// Verifies two objects are not equal
        /// </summary>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expected">The expected value to test against</param>
        /// <param name="userMessage">The message to show when the objects are equal</param>
        public static void NotEqual(object expected, object actual, string userMessage = null)
        {
            if (actual.Equals(expected))
            {
                var msg = CreateFailedMessage($"Assert.NotEqual() Failure. Expected {expected}, Actual {actual}", userMessage);

                throw new TestFailedException(msg);
            }
        }

        /// <summary>
        /// Verifies an Action throws the specified type of exception
        /// </summary>
        /// <param name="testCode">Action to execute</param>
        /// <param name="userMessage">The message to show if the Action does not throw the expected exception</param>
        public static void Throws<T>(Action testCode, string userMessage = null)
            where T : Exception
        {
            try
            {
                testCode();
            }
            catch(T)
            {
                // this is expected, all good
                return;
            }

            var msg = CreateFailedMessage($"Expected exception of type {typeof(T).Name}", userMessage);

            throw new TestFailedException(msg);
        }
    }
}
