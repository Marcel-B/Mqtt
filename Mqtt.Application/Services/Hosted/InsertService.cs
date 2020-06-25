using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace com.b_velop.Mqtt.Application.Services.Hosted
{
    public class InsertService : IHostedService
    {
        private readonly ILogger<InsertService> _logger;
        private readonly IMqttRepository _repo;
        private IHostApplicationLifetime _appLifetime;
        private Timer _timer;
        private static bool Running = false;

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
            _timer = new Timer(InsertValues, null, TimeSpan.FromSeconds(5),
                TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        public DateTime Now()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour,
                DateTime.Now.Minute, 0);
        }

        public async void InsertValues(object state)
        {
            if (Running) return;
            Running = true;
            var lastTime = _repo.LastTimestamp();
            var time = lastTime.AddMinutes(1);
            var mvs = new List<MeasureValue>();
            while (time < Now())
            {
                var t = _repo.AddTimestamp(time);
                time = t.Timestamp;
                time = time.AddMinutes(1);
                
                var msgs = _repo.GetMessagesAsQueryable()
                    .Where(msg => msg.Created >= time && msg.Created < time.AddMinutes(1)).ToList();
                
                var messages = msgs
                    .Where(msg => msg.Topic.StartsWith("arduino"))
                    .Where(msg => !msg.Topic.EndsWith("neo7m")).ToList();

                foreach (var message in messages)
                {
                    if (!double.TryParse(message.Message, out var value))
                    {
                        _repo.DeleteMessage(message);
                        continue;
                    }

                    var fields = message.Topic.Split('/');
                    if (fields.Length != 4)
                    {
                        _repo.DeleteMessage(message);
                        continue;
                    }

                    var room = _repo.GetRoom(fields[1]);
                    // if(room.Name == "mobilehome")
                    //     continue;
                    var mType = _repo.GetMeasureType(fields[2]);
                    var sType = _repo.GetSensorType(fields[3]);
                    if (mvs.FirstOrDefault(vv =>
                        vv.RoomName == room.Name &&
                        vv.MeasureTimeTimestamp == t.Timestamp &&
                        vv.MeasureTypeName == mType.Name &&
                        vv.SensorTypeName == sType.Name) == null)
                    {
                        var m = new MeasureValue
                        {
                            MeasureTimeTimestamp = t.Timestamp,
                            RoomName = room.Name,
                            Value = value,
                            MeasureTypeName = mType.Name,
                            SensorTypeName = sType.Name
                        };
                        mvs.Add(m);
                    }

                    _repo.DeleteMessage(message);
                }

                _repo.AddMeasureValues(mvs);
                await _repo.SaveChangesAsync();
                mvs.Clear();
            }

            await _repo.SaveChangesAsync();
            Running = false;
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