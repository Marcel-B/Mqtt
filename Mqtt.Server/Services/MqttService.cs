using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace com.b_velop.Mqtt.Server.Services
{
    public class MqttService : IHostedService
    {
        private readonly ILogger<MqttService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _appLifetime;

        public MqttService(
            ILogger<MqttService> logger,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _appLifetime = appLifetime;
        }

        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);
            
            return Task.CompletedTask;
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

        private void RunJob(
            object state)
        {

        }
    }
}