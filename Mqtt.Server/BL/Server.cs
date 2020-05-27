using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using com.b_velop.Mqtt.Server.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;

namespace com.b_velop.Mqtt.Server.BL
{
    public class Server : IServer
    {
        private readonly IConfiguration _configuration;
        private readonly IMqttServerStorage _storage;
        private readonly IMqttServerFactory _factory;
        private readonly ILogger<Server> _logger;

        public Server(
            IConfiguration configuration,
            IMqttServerStorage storage,
            IMqttServerFactory factory,
            ILogger<Server> logger)
        {
            _configuration = configuration;
            _storage = storage;
            _factory = factory;
            _logger = logger;
        }

        public MqttServerOptionsBuilder GetOptionsBuilder(
            IEnumerable<User> users,
            IMqttServerSubscriptionInterceptor interceptor, 
            IMqttServerApplicationMessageInterceptor messageInterceptor)
        {
            if (!int.TryParse(_configuration["Settings:Port"], out var port))
                port = 1833;
            
            var us = users.ToList();
            if (us.Count == 0)
            {
                var ulla = _configuration.GetSection("Users").Get<User[]>();
                us.AddRange(ulla);
            }

            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(port)
                .WithStorage(_storage)
                .WithPersistentSessions()
                .WithConnectionBacklog(15)
                //.WithEncryptionSslProtocol(SslProtocols.Tls)
                //.WithEncryptedEndpoint()
                //.WithEncryptedEndpointPort(port)
                
                .WithConnectionValidator(context =>
                {
       
                }).WithSubscriptionInterceptor(interceptor)
                .WithApplicationMessageInterceptor(messageInterceptor);;
            return optionsBuilder;
        }

        public IMqttServer GetServer()
            => _factory.CreateMqttServer();

 
    }
}