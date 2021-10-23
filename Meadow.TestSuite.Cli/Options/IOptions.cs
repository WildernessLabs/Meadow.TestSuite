namespace Meadow.TestSuite
{
    public interface IOptions
    {
        string Port { get; }
        int BaudRate { get; }
        string Ethernet { get; }
    }
}
