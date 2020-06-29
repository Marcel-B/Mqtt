using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Application.Helpers;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace com.b_velop.Mqtt.Application.Services.Hosted
{
    public class InsertService : IHostedService
    {
        private readonly ILogger<InsertService> _logger;
        private readonly IServiceProvider _services;
        private Timer _timer;
        private static bool _running = false;
        private readonly IHostApplicationLifetime _appLifetime;

        public InsertService(
            ILogger<InsertService> logger,
            IServiceProvider services,
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _services = services;
            _appLifetime = appLifetime;

            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                InsertValues,
                null,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private DateTime Now()
        {
            return new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                0);
        }

        private async void InsertValues(object state)
        {
    
            if (_running)
            {
                _logger.LogInformation(4444, $"Job is already running");
                return;
            }

            _running = true;
            var scope = _services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IMqttRepository>();
            var lastTimestamp = repo.LastTimestamp();
            var currentTimestamp = lastTimestamp.AddMinutes(1);

            while (currentTimestamp < Now())
            {
                var timestamp = repo.AddTimestamp(currentTimestamp);
                currentTimestamp = timestamp.Timestamp.AddMinutes(1);

                var messages = await
                    repo.GetMessagesAsQueryable()
                        .Where(msg => msg.Created >= currentTimestamp && msg.Created < currentTimestamp.AddMinutes(1))
                        .Where(msg => msg.Topic.StartsWith("arduino"))
                        .Where(msg => !msg.Topic.EndsWith("neo7m"))
                        .ToListAsync();
                await InsertMessages(repo, messages, timestamp);
            }

            // Check if old messages are present
            var mqttMessages = await
                repo.GetMessagesAsQueryable()
                    .Where(msg => msg.Topic.StartsWith("arduino"))
                    .Where(msg => !msg.Topic.EndsWith("neo7m"))
                    .OrderBy(msg => msg.Created)
                    .ToListAsync();

            foreach (var message in mqttMessages)
            {
                message.Created = message.Created.CutSeconds();
            }

            var newMessages =
                from message in mqttMessages
                group message by message.Created
                into newGroup
                orderby newGroup.Key
                select newGroup;

            var mMessages = new List<MqttMessage>();
            foreach (var messageGroup in newMessages)
            {
                var timestamp = repo.GetTimestamp(messageGroup.Key);
                mMessages.AddRange(messageGroup);
                await InsertMessages(repo, mMessages, timestamp);
                mMessages.Clear();
            }
            _running = false;
        }

        private async Task InsertMessages(
            IMqttRepository repo,
            List<MqttMessage> messages,
            MeasureTime timestamp)
        {
            var measureValues = new List<MeasureValue>();
            foreach (var message in messages)
            {
                if (!message.TryGetFields(out var fields))
                {
                    repo.DeleteMessage(message);
                    continue;
                }

                if (!double.TryParse(message.Message, out var value))
                {
                    repo.DeleteMessage(message);
                    continue;
                }

                var room = repo.GetRoom(fields[1]);
                var measureType = repo.GetMeasureType(fields[2]);
                var sensorType = repo.GetSensorType(fields[3]);

                if (repo.MeasureExists(room.Name, measureType.Name, sensorType.Name, timestamp.Timestamp))
                {
                    repo.DeleteMessage(message);
                }
                if (measureValues.FirstOrDefault(x =>
                    x.RoomName == room.Name &&
                    x.MeasureTimeTimestamp == timestamp.Timestamp &&
                    x.MeasureTypeName == measureType.Name &&
                    x.SensorTypeName == sensorType.Name) == null)
                {
                    var measureValue = new MeasureValue
                    {
                        MeasureTimeTimestamp = timestamp.Timestamp,
                        RoomName = room.Name,
                        Value = value,
                        MeasureTypeName = measureType.Name,
                        SensorTypeName = sensorType.Name
                    };
                    measureValues.Add(measureValue);
                }
                repo.DeleteMessage(message);
            }

            repo.AddMeasureValues(measureValues);
            await repo.SaveChangesAsync();
            measureValues.Clear();
        }

        public Task StopAsync(CancellationToken cancellationToken)
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