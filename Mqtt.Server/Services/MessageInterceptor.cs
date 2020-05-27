using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Server.Services
{
    public class MessageInterceptor : IMqttServerApplicationMessageInterceptor
    {
        private readonly ILogger<MessageInterceptor> _logger;

        public MessageInterceptor(ILogger<MessageInterceptor> logger)
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