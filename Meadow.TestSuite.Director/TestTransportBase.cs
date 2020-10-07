using System;
using Meadow.TestSuite;

namespace Meadow.TestSuite
{
    public abstract class TestTransportBase<TSerializer> : ITestTransport
        where TSerializer : ICommandSerializer
    {
        protected TSerializer Serializer { get; }

        public abstract void DeliverCommand(TestCommand command);

        public TestTransportBase()
        {
            Serializer = Activator.CreateInstance<TSerializer>();
        }

    }
}
