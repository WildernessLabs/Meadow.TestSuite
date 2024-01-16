using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net.Http;

using SoakTests.Common;

namespace SoakTests;

/// <summary>
/// Perform the Socket-based soak test (async).
/// </summary>
class SocketAsyncSoakTest : ISoakTest
{
    /// <summary>
    /// Soak test configuration object.
    /// </summary>
    SoakTestSettings _config;

    /// <summary>
    /// End point to connect to.
    /// </summary>
    private IPEndPoint _ipEndPoint;

    /// <summary>
    /// Request encoded as a byte buffer.
    /// </summary>
    private byte[] _encodedRequest;

    /// <summary>
    /// Setup the test.
    /// </summary>
    /// <param name="config">General soak test configuration</param>
    public void Initialize(SoakTestSettings config)
    {
        _config = config;

        var uri = new Uri(_config.RequestUri, UriKind.Absolute);

        _ipEndPoint = Helpers.GetEndpoint(_config.RequestUri);

        _encodedRequest = Encoding.UTF8.GetBytes($"GET {uri.AbsolutePath} HTTP/1.1\n\n");
    }

    /// <summary>
    /// Execute the test once.
    /// </summary>
    public async Task Execute()
    {
        using Socket client = new(_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        client.Connect(_ipEndPoint);
        _ = await client.SendAsync(_encodedRequest, SocketFlags.None);
        var buffer = new byte[16_384];
        var received = await client.ReceiveAsync(buffer, SocketFlags.None);

        var response = Encoding.UTF8.GetString(buffer, 0, received);
        Console.WriteLine($"Response: {response}");

        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    /// <summary>
    /// Perform any necessary cleanup at the end of the test run.
    /// </summary>
    public void Teardown()
    {
    }
}
