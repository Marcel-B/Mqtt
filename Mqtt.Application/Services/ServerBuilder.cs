using System.Collections.Generic;
using System.Linq;
using com.b_velop.Mqtt.Application.Contracts;
using com.b_velop.Mqtt.Data.Contracts;
using com.b_velop.Mqtt.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Application.Services
{
    public class ServerBuilder : IServerBuilder
    {
        private readonly IConfiguration _configuration;
        private readonly IMqttRepository _repo;
        private readonly IMqttServerStorage _storage;
        private readonly IMqttServerFactory _factory;
        private readonly IMqttServerConnectionValidator _validator;
        private readonly ILogger<ServerBuilder> _logger;

        public ServerBuilder(
            IConfiguration configuration,
            IMqttRepository repo,
            IMqttServerStorage storage,
            IMqttServerFactory factory,
            IMqttServerConnectionValidator validator,
            ILogger<ServerBuilder> logger)
        {
            _configuration = configuration;
            _repo = repo;
            _storage = storage;
            _factory = factory;
            _validator = validator;
            _logger = logger;
        }

        public MqttServerOptionsBuilder GetOptionsBuilder(
            IMqttServerSubscriptionInterceptor interceptor,
            IMqttServerApplicationMessageInterceptor messageInterceptor)
        {
            if (!int.TryParse(_configuration["Settings:Port"], out var port))
                port = 1833;

            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(port)
                .WithStorage(_storage)
                .WithPersistentSessions()
                .WithConnectionBacklog(15)
                //.WithEncryptionSslProtocol(SslProtocols.Tls)
                //.WithEncryptedEndpoint()
                //.WithEncryptedEndpointPort(port)
                .WithConnectionValidator(_validator)
                .WithSubscriptionInterceptor(interceptor)
                .WithApplicationMessageInterceptor(messageInterceptor);
            return optionsBuilder;
        }

        public IMqttServer GetServer()
            => _factory.CreateMqttServer();
    }
}