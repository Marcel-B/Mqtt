namespace com.b_velop.Mqtt.Server.BL
{
    public class Saved
    {
        public string Topic { get; set; }
        public string Payload { get; set; }
        public int QualityOfServiceLevel { get; set; }
        public bool Retain { get; set; }
        public string ContentType { get; set; }
    }
}