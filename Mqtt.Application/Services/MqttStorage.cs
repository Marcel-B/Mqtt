using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Domain.Models;
using MQTTnet;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Application.Services
{
    public class MqttStorage : IMqttServerStorage
    {
        private readonly IMqttRepository _repo;

        public MqttStorage(IMqttRepository repo)
        {
            _repo = repo;
        }
        
        public  Task SaveRetainedMessagesAsync(
            IList<MqttApplicationMessage> messages)
        {
            foreach (var message in messages)
            {
                var payload = message?.Payload == null ? null : Encoding.UTF8.GetString(message?.Payload);
                var msg = _repo.AddMessage(new MqttMessage
                {
                    Created = DateTime.Now, 
                    Message = payload,
                    Topic = message?.Topic,
                    ContentType = message?.ContentType
                });
            }
            _repo.SaveChanges();
            messages.Clear();
            return Task.CompletedTask;
        }

        public Task<IList<MqttApplicationMessage>> LoadRetainedMessagesAsync()
        {
            var messages = _repo.GetMessages();
            IList<MqttApplicationMessage> lst = messages.Select(m => new MqttApplicationMessage
            {
                Payload = Encoding.UTF8.GetBytes(m.Message),
                Topic = m.Topic,
                Retain = true,
                ContentType = m.ContentType,
            }).ToList();
            return Task.FromResult(lst);
        }
    }
}