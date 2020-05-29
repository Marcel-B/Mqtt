using System;

namespace com.b_velop.Mqtt.Domain.Models
{
    public class MeasureValue
    {
        public double Value { get; set; }
        public virtual MeasureTime MeasureTime { get; set; }
        public virtual MeasureType MeasureType { get; set; }
        public virtual SensorType SensorType { get; set; }
        public virtual Room Room { get; set; }
        
        public DateTime MeasureTimeTimestamp { get; set; }
        public string MeasureTypeName { get; set; }
        public string RoomName { get; set; }
        public string SensorTypeName { get; set; }
    }
}