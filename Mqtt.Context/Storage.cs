using com.b_velop.Mqtt.Domain.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace com.b_velop.Mqtt.Context
{
    public static class Storage
    {
        public static void Seed(DataContext context)
        {
            var hans = context.MqttUsers.Find("Hans");
            if (hans != null) return;
            context.MqttUsers.Add(new MqttUser
            {
                Password = "Test",
                Username = "Hans"
            });
            context.SaveChanges();
        }
    }
}