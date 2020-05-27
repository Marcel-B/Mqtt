namespace com.b_velop.Mqtt.Application.Contracts
{
    public interface ISecretProvider
    {
         string GetSecret(string key);
    }
}