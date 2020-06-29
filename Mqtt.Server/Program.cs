using System;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Application.Contracts;
using com.b_velop.Mqtt.Application.Services;
using com.b_velop.Mqtt.Application.Services.Hosted;
using com.b_velop.Mqtt.Context;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
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
            //context.Database.EnsureDeleted();
            context.Database.Migrate();
            context.SaveChanges();
            Storage.Seed(context);
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    services.AddSingleton<IMqttServerOptions, MqttServerOptions>();
                    services.AddSingleton<IMqttServerFactory, MqttFactory>();
                    services.AddSingleton<IMqttServerStorage, MqttStorage>();
                    services.AddSingleton<IMqttServerSubscriptionInterceptor, MqttServerSubscriptionInterceptor>();
                    services
                        .AddSingleton<IMqttServerApplicationMessageInterceptor, MqttServerApplicationMessageInterceptor
                        >();
                    services.AddSingleton<IMqttServerConnectionValidator, MqttServerConnectionValidator>();
                    services.AddSingleton<IServerBuilder, ServerBuilder>();
                    services.AddScoped<IMqttRepository, MqttRepository>();
                    ISecretProvider sp = new SecretProvider();
                    services.AddSingleton(sp);
                    var stage = Environment.GetEnvironmentVariable("STAGE") ?? "";
                    var connectionString = string.Empty;
                    if (stage == "Development")
                    {
                        connectionString = configuration.GetConnectionString("postgres");
                    }
                    else
                    {
                        var db = sp.GetSecret("database");
                        var host = sp.GetSecret("host");
                        var username = sp.GetSecret("username");
                        var port = sp.GetSecret("port");
                        var pw = sp.GetSecret("postgres_db_password");
                        connectionString = $"Host={host};Port={port};Username={username};Password={pw};Database={db};";
                    }
                    services.AddDbContext<DataContext>(options =>
                    {
                        options.UseNpgsql(connectionString);
                        options.EnableSensitiveDataLogging();
                        options.EnableDetailedErrors();
                        options.EnableServiceProviderCaching();
                    });
                    
                    services.AddHostedService<MqttService>();
                    services.AddHostedService<InsertService>();
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