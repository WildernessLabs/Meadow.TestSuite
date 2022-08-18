namespace Meadow.TestSuite
{
    public delegate void CommandReceivedHandler(ITestListener listener, TestCommand command);

    public interface ITestListener
    {
        event CommandReceivedHandler CommandReceived;
        void StartListening();

        void SendResult(object result);
    }
}