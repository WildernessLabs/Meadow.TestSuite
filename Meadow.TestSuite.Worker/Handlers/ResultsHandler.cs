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
            return new JsonResult(MeadowApp.Worker.Results.GetResults());
        }

        [HttpGet("{testID}")]
        public IActionResult GetResults(string testID)
        {
            if (Guid.TryParse(testID, out Guid id))
            {
                return new JsonResult(MeadowApp.Worker.Results.GetResult(id));
            }
            else
            {
                return new JsonResult(MeadowApp.Worker.Results.GetResults(testID));
            }
        }
    }
}