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
            return new JsonResult(MeadowApp.Worker.Registry.GetTestNames());
        }

        [HttpGet("{testID}")]
        public IActionResult GetTestInfo(string testID)
        {
            var info = MeadowApp.Worker.Provider.GetTest(testID);
            
            if (info == null)
            {
                return new NotFoundResult();
            }

            return new JsonResult(info);
        }

        [HttpPost("{testID}")]
        public IActionResult RunTest(string testID)
        {
            var result = MeadowApp.Worker.ExecuteTest(testID);
            
            return new JsonResult(result);
        }
    }
}