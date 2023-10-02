using Meadow;
using Meadow.Foundation.Web.Maple;
using Meadow.Foundation.Web.Maple.Routing;
using Meadow.Hardware;
using System;
using System.Linq;
using System.Threading;

namespace LongTermMapleServiceTest;

public class TextHandler : RequestHandlerBase
{
    public override bool IsReusable => true;
    private IDigitalOutputPort? _led;
    private Random _random = new Random();

    public TextHandler()
        : base()
    {
        _led = Resolver.Services.Get<IDigitalOutputPort>();
    }

    private string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }

    [HttpGet]
    public IActionResult GetText([FromQuery] int length = 1024)
    {
        if (_led != null)
        {
            // blink an LED to allow us to visually see if it's working
            _led.State = true;
            Thread.Sleep(500);
            _led.State = false;
        }

        return new ContentResult(RandomString(length));
    }
}

public class TelemetryRequestHandler : RequestHandlerBase
{
    public override bool IsReusable => true;
    private IDigitalOutputPort? _led;

    public TelemetryRequestHandler()
        : base()
    {
        _led = Resolver.Services.Get<IDigitalOutputPort>();
    }

    [HttpGet]
    public IActionResult GetTelemetry()
    {
        if (_led != null)
        {
            // blink an LED to allow us to visually see if it's working
            _led.State = true;
            Thread.Sleep(500);
            _led.State = false;
        }

        return new JsonResult(new Telemetry());
    }
}