using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Context;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Domain.Models;

namespace com.b_velop.Mqtt.Data.Repositories
{
    public class MqttRepository : IMqttRepository
    {
        private readonly DataContext _context;

        public MqttRepository(
            DataContext context)
        {
            _context = context;
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

        public bool SaveChanges()
            => _context.SaveChanges() > 0;
        
        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken) > 0;

        public Guid AddMessage(MqttMessage message)
        {
            return _context.MqttMessages.Add(message).Entity.Id;
        }

        public IEnumerable<MqttMessage> GetMessages()
        {
            return _context.MqttMessages.ToList();
        }

        public void AddMeasureValue(MeasureValue measureValue)
        {
            _context.MeasureValues.Add(measureValue);
        }
    }
}