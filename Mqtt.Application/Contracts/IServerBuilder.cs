using System.Collections.Generic;
using com.b_velop.Mqtt.Domain.Models;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Application.Contracts
{
    public interface IServerBuilder
    {
        IMqttServer GetServer();

        public MqttServerOptionsBuilder GetOptionsBuilder(
            IMqttServerSubscriptionInterceptor interceptor,
            IMqttServerApplicationMessageInterceptor messageInterceptor);
    }
}