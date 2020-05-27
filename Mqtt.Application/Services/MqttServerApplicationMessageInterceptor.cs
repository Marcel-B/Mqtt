using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Application.Services
{
    public class MqttServerApplicationMessageInterceptor : IMqttServerApplicationMessageInterceptor
    {
        private readonly ILogger<MqttServerApplicationMessageInterceptor> _logger;

        public MqttServerApplicationMessageInterceptor(ILogger<MqttServerApplicationMessageInterceptor> logger)
        {
            _logger = logger;
        }
        
        public Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext context)
        {
            context.AcceptPublish = true;
            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);
            _logger.LogInformation(
                $"Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage?.Topic},"
                + $" Payload = {payload}, QoS = {context.ApplicationMessage?.QualityOfServiceLevel},"
                + $" Retain-Flag = {context.ApplicationMessage?.Retain}");
            return Task.CompletedTask;
        }
    }
}