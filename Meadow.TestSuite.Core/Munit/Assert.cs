using System;

namespace Munit
{
    public static class Assert
    {
        public static void Fail(string message)
        {
            throw new TestFailedException($"Assert.Fail(): {message}");
        }

        public static void True(bool condition, string userMessage = null)
        {
            if (!condition)
            {
                throw new TestFailedException("Assert.True() Failure", userMessage);
            }
        }

        public static void False(bool condition, string userMessage = null)
        {
            if (condition)
            {
                throw new TestFailedException("Assert.False() Failure", userMessage);
            }
        }

        public static void NotNull(object o, string userMessage = null)
        {
            Console.WriteLine("+NotNull");

            if (o == null)
            {
                throw new TestFailedException("Assert.NotNull() Failure", userMessage);
            }
        }

        public static void Null(object o, string userMessage = null)
        {
            if (o != null)
            {
                throw new TestFailedException("Asssert.Null() Failure", userMessage);
            }
        }

        public static void Equal(object expected, object actual, string userMessage = null)
        {
            if (!actual.Equals(expected))
            {
                throw new TestFailedException($"Assert.Equal() Failure. Expected {expected}, Actual {actual}", userMessage);
            }
        }

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

            throw new TestFailedException($"Expected exception of type {typeof(T).Name}", userMessage);
        }
    }
}
