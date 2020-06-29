namespace com.b_velop.Mqtt.Shared.Contracts
{
    public interface ISecretProvider
    {
        string GetSecret(string key);
    }
}