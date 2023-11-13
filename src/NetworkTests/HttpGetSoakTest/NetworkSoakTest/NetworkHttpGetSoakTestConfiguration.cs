using Meadow;

namespace NetworkHttpGetSoakTest;

public class NetworkHttpGetSoakTestConfiguration : ConfigurableObject
{
    public string TestURL => GetConfiguredString(nameof(TestURL), "http://postman-echo.com/get?foo1=bar1&foo2=bar2");

    public int Iterations => GetConfiguredInt(nameof(Iterations), 1000);
}