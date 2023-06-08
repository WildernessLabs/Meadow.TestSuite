using Meadow;
using Meadow.Foundation.Web.Maple;
using Meadow.Foundation.Web.Maple.Routing;
using Meadow.Hardware;
using System.Threading;

namespace LongTermMapleServiceTest;

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