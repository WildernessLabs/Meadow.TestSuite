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

            return new JsonResult(MeadowApp.Worker.Registry.GetTestNames());
        }

        [HttpPost("{testID}")]
        public IActionResult RunTest(string testID)
        {
            Console.WriteLine("Post Test");

            var result = MeadowApp.Worker.ExecuteTest(testID);
            
            return new JsonResult(result);
        }
    }
}