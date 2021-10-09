using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;

namespace MeadowApp
{
    public class ResultsHandler : RequestHandlerBase
    {
        [HttpGet]
        public IActionResult GetResults()
        {
            Console.WriteLine("GET Results");

            // TODO

            return new JsonResult(MeadowApp.Worker.Results.GetResults());
        }

        [HttpGet("{testID}")]
        public IActionResult GetResults(string testID)
        {
            Console.WriteLine("GET Results");

            // TODO

            return new JsonResult(MeadowApp.Worker.Results.GetResults(testID));
        }
    }
}