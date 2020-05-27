using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Server.Services
{
    public class Interceptor : IMqttServerSubscriptionInterceptor
    {
        private readonly ILogger<Interceptor> _logger;

        public Interceptor(ILogger<Interceptor> logger)
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