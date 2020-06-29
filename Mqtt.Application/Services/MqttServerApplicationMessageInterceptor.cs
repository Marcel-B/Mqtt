using System;
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
                if (context.ApplicationMessage.Topic.StartsWith("mcu/"))
                {
                    return;
                }

                var msg = _repo.AddMessage(new MqttMessage
                {
                    Created = DateTime.Now,
                    Message = payload,
                    Topic = context.ApplicationMessage.Topic,
                    ContentType = context.ApplicationMessage.ContentType
                });
                await _repo.SaveChangesAsync();
            }

            _logger.LogInformation(
                $"Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage?.Topic},"
                + $" Payload = {payload}, QoS = {context.ApplicationMessage?.QualityOfServiceLevel},"
                + $" Retain-Flag = {context.ApplicationMessage?.Retain}");
        }
    }
}