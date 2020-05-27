using System;
using System.Collections.Generic;

namespace com.b_velop.Mqtt.Domain.Models
{
    public class MeasureTime
    {
        public DateTime Timestamp { get; set; }
        public virtual ICollection<MeasureValue> MeasureValues { get; set; }
    }
}