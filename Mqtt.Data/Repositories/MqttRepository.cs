using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Context;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Domain.Models;
using Microsoft.Extensions.Logging;

namespace com.b_velop.Mqtt.Data.Repositories
{
    public class MqttRepository : IMqttRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<MqttRepository> _logger;

        public MqttRepository(
            DataContext context,
            ILogger<MqttRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public MqttUser GetUser(string username)
        {
            var user = _context.MqttUsers.FirstOrDefault((u => u.Username.Equals(username)));
            return user;
        }

        public MqttUser AddUser(string username, string password)
        {
            var user = _context.MqttUsers.Add(new MqttUser {Username = username, Password = password});
            return user.Entity;
        }

        public bool SaveChanges() => _context.SaveChanges() > 0;

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var success = false;
            try
            {
                success = await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(6666, e, "Error while saving database changes");
            }

            return success;
        }

        public Guid AddMessage(MqttMessage message) => _context.MqttMessages.Add(message).Entity.Id;
        public IEnumerable<MqttMessage> GetMessages() => _context.MqttMessages.ToList();
        public void AddMeasureValue(MeasureValue measureValue) => _context.MeasureValues.Add(measureValue);

        public MeasureTime AddTimestamp()
        {
            var d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                DateTime.Now.Hour, DateTime.Now.Minute, 0);

            var v = _context.MeasureTimes.FirstOrDefault(t => t.Timestamp == d);

            if (v != null)
                return v;

            v = _context.MeasureTimes.Add(new MeasureTime {Timestamp = d}).Entity;
            try
            {
                _context.SaveChanges();
                return v;
            }
            catch (Exception e)
            {
                _logger.LogError(6666, e, $"Error while saving new Timestamp");
                return null;
            }
        }
    }
}