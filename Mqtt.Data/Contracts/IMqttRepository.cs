using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Domain.Models;

namespace com.b_velop.Mqtt.Data.Contracts
{
    public interface IMqttRepository
    {
        MqttUser GetUser(string username);
        MqttUser AddUser(string username, string password);
        bool SaveChanges();
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
        Guid AddMessage(MqttMessage message);
        IEnumerable<MqttMessage> GetMessages();
        void AddMeasureValue(MeasureValue measureValue);
        DateTime AddTimestamp();
    }
}