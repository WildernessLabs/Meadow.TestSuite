using System;

namespace Munit
{
    public static class Assert
    {
        internal static bool Throw { get; set; } = false;

        public static void Fail(string message)
        {
            if (Assert.Throw)
            {
                throw new TestFailedException($"Assert.Fail(): {message}");
            }
            else
            {
                Console.WriteLine($"Assert.Fail(): {message}");
            }
        }

        public static void True(bool condition, string userMessage = null)
        {
            if (!condition)
            {
                if (Assert.Throw)
                {
                    throw new TestFailedException("Assert.True() Failure", userMessage);
                }
                else
                {
                    Console.WriteLine("Assert.True() Failure", userMessage);
                }
            }
        }

        public static void False(bool condition, string userMessage = null)
        {
            if (condition)
            {
                if (Assert.Throw)
                {
                    throw new TestFailedException("Assert.False() Failure", userMessage);
                }
                else
                {
                    Console.WriteLine("Assert.False() Failure", userMessage);
                }
            }
        }

        public static void NotNull(object o, string userMessage = null)
        {
            if (o == null)
            {
                if (Assert.Throw)
                {
                    throw new TestFailedException("Assert.NotNull() Failure", userMessage);
                }
                else
                {
                    Console.WriteLine("Assert.NotNull() Failure", userMessage);
                }
            }
        }

        public static void Null(object o, string userMessage = null)
        {
            if (o != null)
            {
                if (Assert.Throw)
                {
                    throw new TestFailedException("Assert.Null() Failure", userMessage);
                }
                else
                {
                    Console.WriteLine("Assert.Null() Failure", userMessage);
                }
            }
        }

        public static void Equal(object expected, object actual, string userMessage = null)
        {
            if (!actual.Equals(expected))
            {
                if (Assert.Throw)
                {
                    throw new TestFailedException($"Assert.Equal() Failure. Expected {expected}, Actual {actual}", userMessage);
                }
                else
                {
                    Console.WriteLine($"Assert.Equal() Failure. Expected {expected}, Actual {actual}", userMessage);
                }
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

            if (Assert.Throw)
            {
                throw new TestFailedException($"Expected exception of type {typeof(T).Name}", userMessage);
            }
            else
            {
                Console.WriteLine($"Expected exception of type {typeof(T).Name}", userMessage);
            }
        }
    }
}
