using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Domain.Models;
using Microsoft.Extensions.Logging;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Application.Services
{
    public class MqttServerApplicationMessageInterceptor : IMqttServerApplicationMessageInterceptor
    {
        private readonly IMqttRepository _repo;
        private readonly ILogger<MqttServerApplicationMessageInterceptor> _logger;

        public MqttServerApplicationMessageInterceptor(
            IMqttRepository repo,
            ILogger<MqttServerApplicationMessageInterceptor> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext context)
        {
            context.AcceptPublish = true;
            if (context.ApplicationMessage == null)
                return;

            var payload = context.ApplicationMessage.Payload == null
                ? null
                : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);
            if (context.ApplicationMessage.Retain)
            {
                var msg = _repo.AddMessage(new MqttMessage
                {
                    Created = DateTime.Now,
                    Message = payload,
                    Topic = context.ApplicationMessage.Topic,
                    ContentType = context.ApplicationMessage.ContentType
                });
                var tree = context.ApplicationMessage.Topic.Split('/');
                if (tree.First() == "arduino")
                {
                    var measureTime = _repo.AddTimestamp();
                    if (measureTime != null)
                    {
                        var mv = new MeasureValue
                        {
                            RoomName = tree[1],
                            MeasureTypeName = tree[2],
                            SensorTypeName = tree[3],
                            MeasureTime = measureTime,
                        };
                        if (double.TryParse(payload, out var val))
                        {
                            mv.Value = val;
                            _repo.AddMeasureValue(mv);
                        }
                    }
                }

                try
                {
                    if (await _repo.SaveChangesAsync())
                        context.AcceptPublish = true;
                }
                catch (Exception e)
                {
                    _logger.LogError(6666, e, $"Error while persisting mqtt value to db");
                }
            }

            _logger.LogInformation(
                $"Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage?.Topic},"
                + $" Payload = {payload}, QoS = {context.ApplicationMessage?.QualityOfServiceLevel},"
                + $" Retain-Flag = {context.ApplicationMessage?.Retain}");
        }
    }
}