using System;

namespace Reserve.API.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime SetTime(this DateTime dt, TimeSpan ts)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, ts.Hours, ts.Minutes, ts.Seconds);
        }
    }
}