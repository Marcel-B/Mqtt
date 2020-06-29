﻿using System;
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
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                //context.Database.EnsureDeleted();
                await context.Database.MigrateAsync();
                await context.SaveChangesAsync();
                Storage.Seed(context);
            }

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    
                    services.AddTransient<IMqttServerOptions, MqttServerOptions>();
                    services.AddTransient<IMqttServerFactory, MqttFactory>();
                    services.AddTransient<IMqttServerStorage, MqttStorage>();
                    services.AddTransient<IMqttServerSubscriptionInterceptor, MqttServerSubscriptionInterceptor>();
                    
                    services
                        .AddTransient<IMqttServerApplicationMessageInterceptor, MqttServerApplicationMessageInterceptor
                        >();
                    
                    services.AddTransient<IMqttServerConnectionValidator, MqttServerConnectionValidator>();
                    services.AddTransient<IServerBuilder, ServerBuilder>();
                    services.AddTransient<IMqttRepository, MqttRepository>();
                    
                    ISecretProvider secretProvider = new SecretProvider();
                    services.AddSingleton(secretProvider);
                    
                    var stage = Environment.GetEnvironmentVariable("STAGE") ?? "";
                    var connectionString = string.Empty;
                    
                    if (stage == "Development")
                    {
                        connectionString = configuration.GetConnectionString("postgres");
                    }
                    else
                    {
                        var db = secretProvider.GetSecret("database");
                        var host = secretProvider.GetSecret("host");
                        var username = secretProvider.GetSecret("username");
                        var port = secretProvider.GetSecret("port");
                        var pw = secretProvider.GetSecret("postgres_db_password");
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
                    // services.AddHostedService<InsertService>();
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