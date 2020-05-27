using System.Threading.Tasks;
using com.b_velop.Mqtt.Context;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Data.Repositories;
using com.b_velop.Mqtt.Server.BL;
using com.b_velop.Mqtt.Server.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Server;
using NLog.Extensions.Hosting;

namespace com.b_velop.Mqtt.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMqttServerOptions, MqttServerOptions>();
                    services.AddSingleton<IMqttServerFactory, MqttFactory>();
                    services.AddSingleton<IMqttServerStorage, AwesomeStorage>();
                    services.AddSingleton<IMqttServerSubscriptionInterceptor, Interceptor>();
                    services.AddSingleton<IMqttServerApplicationMessageInterceptor, MessageInterceptor>();
                    services.AddSingleton<BL.IServer, BL.Server>();
                    services.AddScoped<IMqttRepository, MqttRepository>();
                    services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("MqttBase"));
                    services.AddHostedService<MqttService>();
                })
                .ConfigureLogging(
                    logging =>
                    {
                        logging.ClearProviders();
                        logging.AddConsole();
                        logging.SetMinimumLevel(LogLevel.Trace);
                    })
                .UseNLog();
    }
}