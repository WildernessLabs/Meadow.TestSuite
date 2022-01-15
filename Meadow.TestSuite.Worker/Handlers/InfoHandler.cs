using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using Meadow.TestSuite;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MeadowApp
{
    public class InfoHandler : RequestHandlerBase
    {
        public override bool IsReusable => true;

        [HttpGet("/")]
        public IActionResult GetInfo()
        {
            MeadowApp.Worker?.Logger.Debug($"REST: InfoHandler.GetInfo()");

            return new JsonResult(
                new WorkerInfo
                {
                    Name = "Meadow TestSuite",
                    Version = Assembly.GetEntryAssembly().GetName().Version.ToString(3),
                    DateTime = DateTime.UtcNow
                });
        }

        [HttpPut("/debug")]
        public IActionResult ToggleDebug()
        {
            var change = !MeadowApp.Worker.EnableDebugging;
            MeadowApp.Worker.EnableDebugging = change;

            MeadowApp.Worker?.Logger.Info($"Changed debugging to {change}");

            return this.Ok();
        }

        [HttpPost("/reset")]
        public IActionResult ResetDevice()
        {
            MeadowApp.Worker?.Logger.Info($"Resetting Meadow");

            // give enough time for Maple to send a response and the client to receive it
            MeadowApp.Device.WatchdogEnable(TimeSpan.FromSeconds(2));

            return this.Ok();
        }
    }
}