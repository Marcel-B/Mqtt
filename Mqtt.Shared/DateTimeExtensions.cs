using System;

namespace com.b_velop.Mqtt.Shared
{
    public static class DateTimeExtensions
    {
        public static DateTime CutSeconds(this DateTime dateTime) =>
            new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
    }
}