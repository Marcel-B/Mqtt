using System;

namespace com.b_velop.Mqtt.Shared
{
    public static class DateTimeExtensions
    {
        public static DateTime CutSeconds(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
    }
}