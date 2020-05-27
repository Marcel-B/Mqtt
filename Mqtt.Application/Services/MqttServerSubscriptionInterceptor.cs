using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Application.Services
{
    public class MqttServerSubscriptionInterceptor : IMqttServerSubscriptionInterceptor
    {
        private readonly ILogger<MqttServerSubscriptionInterceptor> _logger;

        public MqttServerSubscriptionInterceptor(ILogger<MqttServerSubscriptionInterceptor> logger)
        {
            _logger = logger;
        }
        
        public Task InterceptSubscriptionAsync(
            MqttSubscriptionInterceptorContext context)
        {
            if (context == null)
            {
                return Task.CompletedTask;
            }

            context.AcceptSubscription = true;
            _logger.LogInformation($"New subscription: ClientId = {context.ClientId}, TopicFilter = {context.TopicFilter}");
            return Task.CompletedTask;
        }
    }
}