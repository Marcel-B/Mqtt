using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;

namespace com.b_velop.Mqtt.Client
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var factory = new  MqttFactory();
            var l = factory.CreateMqttClient();
            var opt = new MqttClientOptions();
            
            opt.ClientId = "LocalTestClient";
            opt.ChannelOptions = new MqttClientTcpOptions
            {
                Port = 8883,
                // TlsOptions = new MqttClientTlsOptions
                // {
                //     AllowUntrustedCertificates = true,
                //     UseTls = true,
                // },
                Server = "localhost",
                // Server = "mqtt.qaybe.de",

            };
            opt.Credentials = new MqttClientCredentials
            {
                Username = "Hans",
                Password = Encoding.UTF8.GetBytes("Test")
            };

            var result = await l.ConnectAsync(opt, CancellationToken.None);

            var mssg = new MqttApplicationMessageBuilder();
            mssg.WithRetainFlag(true);
           var la = mssg.WithPayload("Hello from Client sowieso")
                .WithTopic("test/nace/message")
                .Build();
           var response = await l.PublishAsync(la, CancellationToken.None);
           Console.WriteLine(response.ReasonString);
        }
    }
}