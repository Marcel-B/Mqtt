using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;

namespace ConsoleApp1
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var factory = new  MqttFactory();
            var l = factory.CreateMqttClient();
            var opt = new MqttClientOptions();
            opt.ChannelOptions = new MqttClientTcpOptions
            {
Port = 1883,
Server = "localhost"
            };
            opt.Credentials = new MqttClientCredentials
            {
                Username = "Hans",
                Password = Encoding.UTF8.GetBytes("Test")
            };

            var result = await l.ConnectAsync(opt, CancellationToken.None);
            Console.WriteLine(result.ReasonString);
            Console.WriteLine(l.IsConnected);
            var mssg = new MqttApplicationMessageBuilder();
           var la = mssg.WithPayload("Hello from Client sowieso")
                .WithTopic("foo")
                .Build();
           var response = await l.PublishAsync(la, CancellationToken.None);
           Console.WriteLine(response.ReasonString);
        }
    }
}