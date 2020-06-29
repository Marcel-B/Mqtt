using com.b_velop.Mqtt.Domain.Models;

namespace com.b_velop.Mqtt.Shared
{
    public static class MqttExtension
    {
        public static bool TryGetFields(this MqttMessage message, out string[] fields)
        {
            fields = message.Topic.Split('/');
            return fields.Length == 4;
        }
    }
}