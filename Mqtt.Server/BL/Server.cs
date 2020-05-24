using Microsoft.Extensions.Logging;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Server.BL
{
    public class Server
    {
        private readonly IMqttServerFactory _factory;
        private readonly ILogger<Server> _logger;

        public Server(
            IMqttServerFactory factory,
            ILogger<Server> logger)
        {
            _factory = factory;
            _logger = logger;
        }
        public IMqttServer Build()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1833)
                .WithConnectionValidator(
                    c =>
                    {
                        var currentUser = config.Users.FirstOrDefault(u => u.UserName == c.Username);

                        if (currentUser == null)
                        {
                            c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                            LogMessage(c, true);
                            return;
                        }

                        if (c.Username != currentUser.UserName)
                        {
                            c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                            LogMessage(c, true);
                            return;
                        }

                        if (c.Password != currentUser.Password)
                        {
                            c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                            LogMessage(c, true);
                            return;
                        }

                        c.ReasonCode = MqttConnectReasonCode.Success;
                        LogMessage(c, false);
                    }).WithSubscriptionInterceptor(
                    c =>
                    {
                        c.AcceptSubscription = true;
                        LogMessage(c, true);
                    }).WithApplicationMessageInterceptor(
                    c =>
                    {
                        c.AcceptPublish = true;
                        LogMessage(c);
                    });

            var mqttServer = _factory.CreateMqttServer();
            
            return mqttServer;
            
            // await mqttServer.StartAsync(optionsBuilder.Build());
        }
    }
}