using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("App")]

namespace Munit
{
    public static class Assert
    {
        internal static bool ThrowOnFail { get; set; } = false;
        internal static bool HasFailed { get; set; }
        internal static string LastFailMessage { get; set; }

        private static void SetFailed(string baseMessage, string userMessage)
        {
            HasFailed = true;
            LastFailMessage = $"{baseMessage}{(userMessage == null ? string.Empty : $": {userMessage}")}";
        }

        /// <summary>
        /// Forces a test failure
        /// </summary>
        /// <param name="userMessage">The message to show</param>
        public static void Fail(string userMessage)
        {
            SetFailed("Assert.Fail()", userMessage);

            if (Assert.ThrowOnFail)
            {
                throw new TestFailedException(LastFailMessage);
            }
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
                SetFailed("Assert.True() Failure", userMessage);

                if (Assert.ThrowOnFail)
                {
                    throw new TestFailedException(LastFailMessage);
                }
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
                SetFailed("Assert.False() Failure", userMessage);

                if (Assert.ThrowOnFail)
                {
                    throw new TestFailedException(LastFailMessage);
                }
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
                SetFailed("Assert.NotNull() Failure", userMessage);

                if (Assert.ThrowOnFail)
                {
                    throw new TestFailedException(LastFailMessage);
                }
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
                SetFailed("Assert.Null() Failure", userMessage);

                if (Assert.ThrowOnFail)
                {
                    throw new TestFailedException(LastFailMessage);
                }
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
                SetFailed($"Assert.Equal() Failure. Expected {expected}, Actual {actual}", userMessage);

                if (Assert.ThrowOnFail)
                {
                    throw new TestFailedException(LastFailMessage);
                }
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
                SetFailed($"Assert.NotEqual() Failure. Expected {expected}, Actual {actual}", userMessage);

                if (Assert.ThrowOnFail)
                {
                    throw new TestFailedException(LastFailMessage);
                }
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

            SetFailed($"Expected exception of type {typeof(T).Name}", userMessage);

            if (Assert.ThrowOnFail)
            {
                throw new TestFailedException(LastFailMessage);
            }
        }
    }
}
