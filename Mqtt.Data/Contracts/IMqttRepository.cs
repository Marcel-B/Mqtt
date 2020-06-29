using System;
using System.Collections.Generic;
using System.Linq;
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
         IQueryable<MqttMessage> GetMessagesAsQueryable();
        void AddMeasureValue(MeasureValue measureValue);
        MeasureTime AddTimestamp();
        MeasureTime AddTimestamp(DateTime timestamp);
        MeasureTime GetTimestamp(DateTime timestamp);
        void DeleteMessage(MqttMessage message);
        Room GetRoom(string room);
        SensorType GetSensorType(string sensorType);
        MeasureType GetMeasureType(string measureType);
        bool MeasureExsists(string roomName, string measureType, string sensorType, DateTime timestamp);
        DateTime LastTimestamp();
        void AddMeasureValues(List<MeasureValue> measureValues);
    }
}