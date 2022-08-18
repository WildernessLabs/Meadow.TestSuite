using System;

namespace Munit
{
    public class TestFailedException : Exception
    {
        internal TestFailedException(string message)
            : base(message)
        {
        }

        internal TestFailedException(string baseMessage, string userMessage = null)
            : base($"{baseMessage}{(userMessage == null ? string.Empty : $": {userMessage}")}")
        {
        }
    }
}
