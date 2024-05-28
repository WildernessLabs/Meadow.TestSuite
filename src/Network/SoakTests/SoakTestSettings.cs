using Meadow;

namespace SoakTests;

public class SoakTestSettings : ConfigurableObject
{
    /// <summary>
    /// Name of the display to be used to show progress.
    /// </summary>
    /// <returns>Name of the configured display.</returns>
    public string DisplayName => GetConfiguredString(nameof(DisplayName), "None");

    /// <summary>
    /// Name of the test to be executed.
    /// </summary>
    /// <returns>Name of the configured test.</returns>
    public string TestName => GetConfiguredString(nameof(TestName), "None");

    /// <summary>
    /// URI to be used by any tests that need one.
    /// </summary>
    /// <returns>URI to be used by the test.</returns>
    public string RequestUri => GetConfiguredString(nameof(RequestUri), "http://postman-echo.com/get?foo1=bar1");

    /// <summary>
    /// Number of time to execute the test.
    /// </summary>
    /// <returns>Number of cycles to execute the test.</returns>
    public int NumberOfCycles => GetConfiguredInt(nameof(NumberOfCycles), 10000);

    /// <summary>
    /// If there should be a delay between requests, this is the delay in milliseconds.
    /// </summary>
    /// <returns>Delay (in milliseconds) between requests.</returns>
    public int DelayBetweenCyclesMs => GetConfiguredInt(nameof(DelayBetweenCyclesMs), 1000);
}