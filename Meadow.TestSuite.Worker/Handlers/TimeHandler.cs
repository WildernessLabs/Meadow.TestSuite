using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;
using System.IO;

namespace MeadowApp
{
    public class TimeHandler : RequestHandlerBase
    {
        internal class TimeInfo
        {
            public TimeInfo()
            {
                SystemTime = DateTime.UtcNow;
            }

            public DateTime SystemTime { get; set; }
        }

        public override bool IsReusable => true;

        [HttpGet]
        public IActionResult GetTime()
        {
            return new JsonResult(new TimeInfo());
        }

        [HttpPut]
        public IActionResult SetTime()
        {
            try
            {
                string json;

                using (var reader = new StreamReader(Context.Request.InputStream))
                {
                    json = reader.ReadToEnd();
                }

                var info = SimpleJson.SimpleJson.DeserializeObject<TimeInfo>(json);

                if (info.SystemTime != DateTime.MinValue)
                {
                    MeadowApp.Device.SetClock(info.SystemTime);
                    return new OkResult();
                }

                return new StatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                return new ServerErrorResult(ex);
            }
        }
    }
}