using System;
using SoakTests.Common;

namespace SoakTests;

class HttpGetSoakTest : ISoakTest
{
    SoakTestSettings _config;

    public string Name => "HttpGetSoakTest";

    public void Initialize(SoakTestSettings config)
    {
        _config = config;
    }

    public async void Execute()
    {
        await Helpers.GetWebPageViaHttpClient(_config.RequestUri);
    }

    public void Teardown()
    {
    }
}