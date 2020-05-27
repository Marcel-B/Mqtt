using System;
using System.Collections.Generic;
using com.b_velop.Mqtt.Domain.Models;

namespace com.b_velop.Mqtt.Data.Contracts
{
    public interface IMqttRepository
    {
        MqttUser GetUser(string username);
        MqttUser AddUser(string username, string password);
        bool SaveChanges();
        Guid AddMessage(MqttMessage message);
        IEnumerable<MqttMessage> GetMessages();
    }
}