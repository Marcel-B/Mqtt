using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Server.BL;
using com.b_velop.Mqtt.Server.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Server;
using NLog.Fluent;

namespace com.b_velop.Mqtt.Server.Services
{
    public class MqttService : IHostedService
    {
        private readonly ILogger<MqttService> _logger;
        private readonly IMqttServerSubscriptionInterceptor _interceptor;
        private readonly IMqttServerApplicationMessageInterceptor _messageInterceptor;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _appLifetime;

        public MqttService(
            ILogger<MqttService> logger,
            IMqttServerSubscriptionInterceptor interceptor,
            IMqttServerApplicationMessageInterceptor messageInterceptor,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _interceptor = interceptor;
            _messageInterceptor = messageInterceptor;
            _serviceProvider = serviceProvider;
            _appLifetime = appLifetime;
        }

        public async Task StartAsync(
            CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);
            using var scope = _serviceProvider.CreateScope();
            var serverBuilder = scope.ServiceProvider.GetRequiredService<BL.IServer>();
            var options = serverBuilder.GetOptionsBuilder(new List<User>(), _interceptor, _messageInterceptor);

            var server = serverBuilder.GetServer();
            await server.StartAsync(options.Build());
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
            // Perform post-stopped activities here
        }
    }
}