using System;
using com.b_velop.Mqtt.Domain.Models;

namespace com.b_velop.Mqtt.Application.Helpers
{
    public static class DateTimeExtension
    {
        public static DateTime CutSeconds(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
    }

    public static class MqttExtension
    {
        public static bool TryGetFields(this MqttMessage message, out string[] fields)
        {
            fields = message.Topic.Split('/');
            return fields.Length == 4;
        }
    }
}