using System;

namespace Munit
{
    public static class Assert
    {
        // TODO: these need to become some form of object to return from a test
        public static void True(bool condition)
        {
            if (!condition)
            {
                Console.WriteLine("FAILED: Expected True");
            }
        }

        public static void False(bool condition)
        {
            if (condition)
            {
                Console.WriteLine("FAILED: Expected False");
            }
        }

        public static void NotNull(object o)
        {
            if(o == null)
            {
                Console.WriteLine("FAILED: Expected Not Null");
            }
        }

        public static void Null(object o)
        {
            if (o != null)
            {
                Console.WriteLine("FAILED: Expected Null");
            }
        }

        public static void Equal(object expected, object actual)
        {
            if (!actual.Equals(expected))
            {
                Console.WriteLine($"FAILED: Expected {expected}, Actual {actual}");
            }
        }
    }
}
