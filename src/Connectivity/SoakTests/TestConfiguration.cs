using Meadow;

namespace SoakTests;

public class SoakTestSettings : ConfigurableObject
{
    /// <summary>
    /// Name of the test to be executed.
    /// </summary>
    public string TestName => GetConfiguredString(nameof(TestName), "None");

    /// <summary>
    /// URI to be used by any tests that need one.
    /// </summary>
    public string RequestUri => GetConfiguredString(nameof(RequestUri), "http://postman-echo.com/get?foo1=bar1");

    /// <summary>
    /// Number of time to execute the test.
    /// </summary>
    public int NumberOfCycles => GetConfiguredInt(nameof(NumberOfCycles), 10000);

    /// <summary>
    /// If there should be a delay between requests, this is the delay in milliseconds.
    /// </summary>
    public int DelayBetweenCyclesMs => GetConfiguredInt(nameof(DelayBetweenCyclesMs), 1000);
}