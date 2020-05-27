using System.Threading.Tasks;
using com.b_velop.Mqtt.Data.Contracts;
using Microsoft.Extensions.Logging;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Application.Services
{
    public class MqttServerConnectionValidator: IMqttServerConnectionValidator
    {
        private readonly IMqttRepository _repo;
        private readonly ILogger<MqttServerConnectionValidator> _logger;

        public MqttServerConnectionValidator(
            IMqttRepository repo,
            ILogger<MqttServerConnectionValidator> logger)
        {
            _repo = repo;
            _logger = logger;
        }
        
        public Task ValidateConnectionAsync(
            MqttConnectionValidatorContext context)
        {
            
            var currentUser = _repo.GetUser(context.Username);

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