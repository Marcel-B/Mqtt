using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.b_velop.Mqtt.Server.Models;
using Microsoft.Extensions.Logging;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Server.Services
{
    public class ConnectionValidator : IMqttServerConnectionValidator
    {
        private readonly IEnumerable<User> _users;
        private readonly ILogger<ConnectionValidator> _logger;

        public ConnectionValidator(
            IEnumerable<User> users,
            ILogger<ConnectionValidator> logger)
        {
            _users = users;
            _logger = logger;
        }
        
        public Task ValidateConnectionAsync(
            MqttConnectionValidatorContext context)
        {
            
            var currentUser = _users.FirstOrDefault(u => u.Username == context.Username);

            if (currentUser == null)
            {
                context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                LogMessage(context);
                return Task.CompletedTask;
            }

            if (context.Username != currentUser.Username)
            {
                context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                LogMessage(context);
                return Task.CompletedTask;
            }

            if (context.Password != currentUser.Password)
            {
                context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                LogMessage(context);
                return Task.CompletedTask;
            }

            context.ReasonCode = MqttConnectReasonCode.Success;
            LogMessage(context);
            return Task.CompletedTask;
        }
        
        private void LogMessage(MqttConnectionValidatorContext context, bool showPassword = false)
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