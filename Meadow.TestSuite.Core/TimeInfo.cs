using System;

namespace Meadow.TestSuite
{
    public class TimeInfo
    {
        public TimeInfo(DateTime time)
        {
            SystemTime = time;
        }

        public TimeInfo()
        {
            SystemTime = DateTime.UtcNow;
        }

        public DateTime SystemTime { get; set; }
    }
}
