using System;

namespace com.b_velop.Mqtt.Domain.Models
{
    public class MqttMessage
    {
        public Guid Id { get; set; }
        public string Topic { get; set; }
        public string ContentType { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; }
    }
}