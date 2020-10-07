namespace Meadow.TestSuite
{
    public delegate void CommandReceivedHandler(TestCommand command);

    public interface ITestListener
    {
        event CommandReceivedHandler CommandReceived;
        void StartListening();
    }
}