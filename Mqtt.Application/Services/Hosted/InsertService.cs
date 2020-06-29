using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Application.Helpers;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace com.b_velop.Mqtt.Application.Services.Hosted
{
    public class InsertService : IHostedService
    {
        private readonly ILogger<InsertService> _logger;
        private readonly IMqttRepository _repo;
        private Timer _timer;
        private static bool _running = false;
        private readonly IHostApplicationLifetime _appLifetime;

        public InsertService(
            ILogger<InsertService> logger,
            IMqttRepository repo,
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _repo = repo;
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
            var lastTimestamp = _repo.LastTimestamp();
            var currentTimestamp = lastTimestamp.AddMinutes(1);

            while (currentTimestamp < Now())
            {
                var timestamp = _repo.AddTimestamp(currentTimestamp);
                currentTimestamp = timestamp.Timestamp.AddMinutes(1);

                var messages = await
                    _repo.GetMessagesAsQueryable()
                        .Where(msg => msg.Created >= currentTimestamp && msg.Created < currentTimestamp.AddMinutes(1))
                        .Where(msg => msg.Topic.StartsWith("arduino"))
                        .Where(msg => !msg.Topic.EndsWith("neo7m"))
                        .ToListAsync();
                await InsertMessages(messages, timestamp);
            }

            // Check if old messages are present
            var mqttMessages = await
                _repo.GetMessagesAsQueryable()
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
                var timestamp = _repo.GetTimestamp(messageGroup.Key);
                mMessages.AddRange(messageGroup);
                await InsertMessages(mMessages, timestamp);
                mMessages.Clear();
            }
            _running = false;
        }

        private async Task InsertMessages(
            List<MqttMessage> messages,
            MeasureTime timestamp)
        {
            var measureValues = new List<MeasureValue>();
            foreach (var message in messages)
            {
                if (!message.TryGetFields(out var fields))
                {
                    _repo.DeleteMessage(message);
                    continue;
                }

                if (!double.TryParse(message.Message, out var value))
                {
                    _repo.DeleteMessage(message);
                    continue;
                }

                var room = _repo.GetRoom(fields[1]);
                var measureType = _repo.GetMeasureType(fields[2]);
                var sensorType = _repo.GetSensorType(fields[3]);

                if (_repo.MeasureExists(room.Name, measureType.Name, sensorType.Name, timestamp.Timestamp))
                {
                    _repo.DeleteMessage(message);
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
                _repo.DeleteMessage(message);
            }

            _repo.AddMeasureValues(measureValues);
            await _repo.SaveChangesAsync();
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