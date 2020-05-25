using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Server.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;

namespace com.b_velop.Mqtt.Server.BL
{
    public class Saved
    {
        public string Topic { get; set; }
        public string Payload { get; set; }
        public int QualityOfServiceLevel { get; set; }
        public bool Retain { get; set; }
        public string ContentType { get; set; }
    }
    
    public class AwsomeStorage : IMqttServerStorage
    {
        public  Task SaveRetainedMessagesAsync(IList<MqttApplicationMessage> messages)
        {
            // var fi = new FileInfo("save.txt");
            // StreamWriter wr;
            // if (!fi.Exists)
            //     wr = fi.CreateText();
            // else
            //     wr = fi.AppendText();
            //
            // foreach (var message in messages)
            // {
            //     var l =  JsonConvert.SerializeObject(message);
            //     await wr.WriteLineAsync(l);
            // }
            // wr.Close();
            return Task.CompletedTask;
        }

        public Task<IList<MqttApplicationMessage>> LoadRetainedMessagesAsync()
        {
            IList<MqttApplicationMessage> lst = new List<MqttApplicationMessage>();
            return Task.FromResult(lst);
        }
    }
    public interface IServer
    {
        IMqttServer GetServer();
        public MqttServerOptionsBuilder GetOptionsBuilder(IEnumerable<User> users);
    }

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
            IEnumerable<User> users)
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
                .WithConnectionValidator(context =>
                {
                    var currentUser = us.FirstOrDefault(u => u.Username == context.Username);

                    if (currentUser == null)
                    {
                        context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        LogMessage(context, true);
                        return;
                    }

                    if (context.Username != currentUser.Username)
                    {
                        context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        LogMessage(context, true);
                        return;
                    }

                    if (context.Password != currentUser.Password)
                    {
                        context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        LogMessage(context, true);
                        return;
                    }

                    context.ReasonCode = MqttConnectReasonCode.Success;
                    LogMessage(context, false);
                });
            return optionsBuilder;
        }

        public IMqttServer GetServer()
            => _factory.CreateMqttServer();

        private void LogMessage(MqttSubscriptionInterceptorContext context, bool successful)
        {
            if (context == null)
            {
                return;
            }

            _logger.LogInformation(successful
                ? $"New subscription: ClientId = {context.ClientId}, TopicFilter = {context.TopicFilter}"
                : $"Subscription failed for clientId = {context.ClientId}, TopicFilter = {context.TopicFilter}");
        }
        
        private void LogMessage(MqttConnectionValidatorContext context, bool showPassword)
        {
            if (context == null)
            {
                return;
            }

            if (showPassword)
            {
                _logger.LogInformation(
                    $"New connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint},"
                    + $" Username = {context.Username}, Password = {context.Password},"
                    + $" CleanSession = {context.CleanSession}");
            }
            else
            {
                _logger.LogInformation(
                    $"New connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint},"
                    + $" Username = {context.Username}, CleanSession = {context.CleanSession}");
            }
        }
    }
}