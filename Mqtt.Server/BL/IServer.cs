using System.Collections.Generic;
using com.b_velop.Mqtt.Server.Models;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Server.BL
{
    public interface IServer
    {
        IMqttServer GetServer();
        public MqttServerOptionsBuilder GetOptionsBuilder(IEnumerable<User> users, IMqttServerSubscriptionInterceptor interceptor, IMqttServerApplicationMessageInterceptor messageInterceptor);
    }
}