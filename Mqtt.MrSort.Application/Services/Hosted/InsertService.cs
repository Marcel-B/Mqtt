using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Domain.Models;
using com.b_velop.Mqtt.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace com.b_velop.Mqtt.MrSort.Application.Services.Hosted
{
    public class InsertService : IHostedService
    {
        private readonly ILogger<InsertService> _logger;
        private readonly IMqttRepository _repository;
        private Timer _timer;
        private static bool _running = false;
        private readonly IHostApplicationLifetime _appLifetime;

        public InsertService(
            ILogger<InsertService> logger,
            IMqttRepository repository,
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _repository = repository;
            _appLifetime = appLifetime;

            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                InsertValues,
                true,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(3));

            return Task.CompletedTask;
        }

        private async void InsertValues(
            object state)
        {
            if (_running)
            {
                _logger.LogInformation(4444, $"Job is already running");
                return;
            }

            _running = true;

            var lastTimestamp = _repository.LastTimestamp();
            var currentTimestamp = lastTimestamp.AddMinutes(1);

            while (currentTimestamp < DateTime.Now.CutSeconds())
            {
                _ = _repository.AddTimestamp(currentTimestamp.CutSeconds());
                currentTimestamp = currentTimestamp.AddMinutes(1);
            }

            _ = await _repository.SaveChangesAsync();

            var targets = new List<MeasureValue>();
            var delete = new List<MqttMessage>();

            // Filter relevant messages
            var messages =
                await _repository
                    .GetMessagesAsQueryable()
                    .Where(msg => !msg.Topic.EndsWith("neo7m"))
                    .OrderBy(msg => msg.Created)
                    .ToListAsync();

            foreach (var message in messages)
            {
                if (!double.TryParse(message.Message, out var v))
                {
                    delete.Add(message);
                    continue;
                }

                var fields = message.Topic.Split('/');

                if (fields.Length != 4)
                {
                    delete.Add(message);
                    continue;
                }

                var tmpTimestamp = message.Created.CutSeconds();
                var timestamp = _repository.GetTimestamp(tmpTimestamp);
                var room = _repository.GetRoom(fields[1]);
                var measureType = _repository.GetMeasureType(fields[2]);
                var sensorType = _repository.GetSensorType(fields[3]);

                if (room == null && measureType == null && sensorType == null)
                {
                    _logger.LogError(6666, $"Error with types");
                    continue;
                }

                var value = new MeasureValue
                {
                    MeasureTimeTimestamp = timestamp.Timestamp,
                    SensorTypeName = sensorType.Name,
                    RoomName = room?.Name,
                    MeasureTypeName = measureType?.Name,
                    Value = v
                };

                if (targets.FirstOrDefault(
                    msg =>
                        msg.RoomName == value.RoomName &&
                        msg.SensorTypeName == value.SensorTypeName &&
                        msg.MeasureTypeName == value.MeasureTypeName &&
                        msg.MeasureTimeTimestamp == value.MeasureTimeTimestamp) != null)
                {
                    _logger.LogError(4444, $"Value are duplicate in MqttMessages");
                    delete.Add(message);
                    continue;
                }
                targets.Add(value);
                delete.Add(message);
            }

            foreach (var target in targets)
            {
                if (!_repository.MeasureExists(
                    target.RoomName,
                    target.MeasureTypeName,
                    target.SensorTypeName,
                    target.MeasureTimeTimestamp))
                {
                    _repository.AddMeasureValue(target);
                } 
            }

            foreach (var dMessage in delete)
            {
                _repository.DeleteMessage(dMessage);
            }
            _ = await _repository.SaveChangesAsync();
            //
            // var lastTimestamp = _repository.LastTimestamp();
            // var currentTimestamp = lastTimestamp.AddMinutes(1);
            //
            // while (currentTimestamp < DateTime.Now.CutSeconds())
            // {
            //     var timestamp = _repository.AddTimestamp(currentTimestamp);
            //     currentTimestamp = timestamp.Timestamp.AddMinutes(1);
            //
            //     var messages = await
            //         _repository.GetMessagesAsQueryable()
            //             .Where(msg => msg.Created >= currentTimestamp && msg.Created < currentTimestamp.AddMinutes(1))
            //             .Where(msg => msg.Topic.StartsWith("arduino"))
            //             .Where(msg => !msg.Topic.EndsWith("neo7m"))
            //             .ToListAsync();
            //
            //     await InsertMessages(_repository, messages, timestamp);
            // }
            //
            // // Check if old messages are present
            // var mqttMessages = await
            //     _repository.GetMessagesAsQueryable()
            //         .Where(msg => msg.Topic.StartsWith("arduino"))
            //         .Where(msg => !msg.Topic.EndsWith("neo7m"))
            //         .OrderBy(msg => msg.Created)
            //         .ToListAsync();
            //
            // foreach (var message in mqttMessages)
            // {
            //     message.Created = message.Created.CutSeconds();
            // }
            //
            // var newMessages =
            //     from message in mqttMessages
            //     group message by message.Created
            //     into newGroup
            //     orderby newGroup.Key
            //     select newGroup;
            //
            // var mMessages = new List<MqttMessage>();
            // foreach (var messageGroup in newMessages)
            // {
            //     var timestamp = _repository.GetTimestamp(messageGroup.Key);
            //     mMessages.AddRange(messageGroup);
            //     await InsertMessages(_repository, mMessages, timestamp);
            //     mMessages.Clear();
            // }

            _running = false;
        }

        private async Task InsertMessages(
            IMqttRepository repo,
            IEnumerable<MqttMessage> messages,
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