using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;

namespace MeadowApp
{
    public class TestsHandler : RequestHandlerBase
    {
        [HttpGet]
        public IActionResult GetTests()
        {
            Console.WriteLine("GET Tests");

            // TODO:          

            return new JsonResult(AppState.Registry.GetTestNames());
        }

        [HttpPost("testID")]
        public IActionResult RunTest(string testID)
        {
            Console.WriteLine("Post Test");

            // TODO:

            MeadowApp.Worker.ExecuteTest(testID);

            return new JsonResult(AppState.Registry.GetTestNames());
        }
    }
}