using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using Meadow.TestSuite;
using System.Reflection;

namespace MeadowApp
{
    public class InfoHandler : RequestHandlerBase
    {
        public override bool IsReusable => true;

        [HttpGet("/")]
        public IActionResult GetInfo()
        {
            return new JsonResult(
                new WorkerInfo
                {                     
                    Name = "Meadow TestSuite",
                    Version = Assembly.GetEntryAssembly().GetName().Version.ToString(3)
                });
        }
    }
}