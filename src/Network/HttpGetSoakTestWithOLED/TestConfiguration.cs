using Meadow;

namespace FeatherF7Test
{
    public class TestSettings : ConfigurableObject
    {
        public string RequestUri => GetConfiguredString(nameof(RequestUri), "http://postman-echo.com/get?foo1=bar1");
        public int NumberOfRequests => GetConfiguredInt(nameof(NumberOfRequests), 10000);
        public int DelayBetweenRequestsMs => GetConfiguredInt(nameof(DelayBetweenRequestsMs), 1000);
    }
}