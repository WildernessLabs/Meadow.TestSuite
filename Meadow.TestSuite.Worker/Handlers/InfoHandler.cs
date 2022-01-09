using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using Meadow.TestSuite;
using System;
using System.Reflection;

namespace MeadowApp
{
    public class InfoHandler : RequestHandlerBase
    {
        public override bool IsReusable => true;

        [HttpGet("/")]
        public IActionResult GetInfo()
        {
            Console.WriteLine("REST GET at '/'");

            return new JsonResult(
                new WorkerInfo
                {                     
                    Name = "Meadow TestSuite",
                    Version = Assembly.GetEntryAssembly().GetName().Version.ToString(3),
                    DateTime = DateTime.UtcNow
                });
        }
    }
}