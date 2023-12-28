using System;
using SoakTests.Common;

namespace SoakTests;

/// <summary>
/// Perform the async HttpClient-based soak test.
/// </summary>
class HttpGetSoakTest : ISoakTest
{
    /// <summary>
    /// Soak test configuration object.
    /// </summary>
    SoakTestSettings _config;

    /// <summary>
    /// Setup the test.
    /// </summary>
    /// <param name="config">General soak test configuration</param>
    public void Initialize(SoakTestSettings config)
    {
        _config = config;
    }

    /// <summary>
    /// Execute the test once.
    /// </summary>
    public async void Execute()
    {
        await Helpers.GetWebPageViaHttpClient(_config.RequestUri);
    }

    /// <summary>
    /// Perform any necessary cleanup at the end of the test run.
    /// </summary>
    public void Teardown()
    {
    }
}