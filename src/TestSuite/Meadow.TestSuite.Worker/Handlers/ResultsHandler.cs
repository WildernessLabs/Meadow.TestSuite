using Meadow.Foundation.Web.Maple;
using Meadow.Foundation.Web.Maple.Routing;
using System;

namespace MeadowApp
{
    public class ResultsHandler : RequestHandlerBase
    {
        public override bool IsReusable => true;

        [HttpGet]
        public IActionResult GetResults()
        {
            MeadowApp.Worker?.Logger.Debug($"REST: ResultsHandler.GetResults()");

            return new JsonResult(MeadowApp.Worker.Results.GetResults());
        }

        [HttpGet("{testID}")]
        public IActionResult GetResults(string testID)
        {
            MeadowApp.Worker?.Logger.Debug($"REST: ResultsHandler.GetResults({testID})");

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