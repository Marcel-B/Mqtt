using System.Threading.Tasks;
using com.b_velop.Mqtt.Application.Contracts;
using com.b_velop.Mqtt.Application.Services;
using com.b_velop.Mqtt.Application.Services.Hosted;
using com.b_velop.Mqtt.Context;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Data.Repositories;
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
            var context = host.Services.GetRequiredService<DataContext>();
            context.Database.Migrate();
            context.SaveChanges();
            Storage.Seed(context);
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMqttServerOptions, MqttServerOptions>();
                    services.AddSingleton<IMqttServerFactory, MqttFactory>();
                    services.AddSingleton<IMqttServerStorage, MqttStorage>();
                    services.AddSingleton<IMqttServerSubscriptionInterceptor, MqttServerSubscriptionInterceptor>();
                    services.AddSingleton<IMqttServerApplicationMessageInterceptor, MqttServerApplicationMessageInterceptor>();
                    services.AddSingleton<IMqttServerConnectionValidator, MqttServerConnectionValidator>();
                    services.AddSingleton<IServerBuilder, ServerBuilder>();
                    services.AddScoped<IMqttRepository, MqttRepository>();
                    services.AddDbContext<DataContext>(options => options.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=password;Database=MqttBase;"));
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