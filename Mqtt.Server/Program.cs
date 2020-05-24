using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args);
            await host.RunConsoleAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
           => Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
           {
               services.AddScoped<IMqttServerOptions, MqttServerOptions>();
               services.AddScoped<IMqttServerFactory, MqttFactory>();
           });
    }
}