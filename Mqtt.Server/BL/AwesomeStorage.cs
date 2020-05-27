using System.Collections.Generic;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;

namespace com.b_velop.Mqtt.Server.BL
{
    public class AwesomeStorage : IMqttServerStorage
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
}