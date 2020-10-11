using System;
using Meadow.TestSuite;

namespace Meadow.TestSuite
{
    public abstract class TestTransportBase : ITestTransport
    {
        protected ICommandSerializer Serializer { get; }

        public abstract byte[] DeliverCommand(TestCommand command);

        public TestTransportBase(ICommandSerializer serializer)
        {
            Serializer = serializer;
        }

    }
}
