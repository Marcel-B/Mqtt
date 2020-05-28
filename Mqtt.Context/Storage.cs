using System;
using System.Linq;
using com.b_velop.Mqtt.Domain.Models;

namespace com.b_velop.Mqtt.Context
{
    public static class Storage
    {
        public static void Seed(DataContext context)
        {
            var hans = context.MqttUsers.Find("Hans");
            if (hans == null)
                context.MqttUsers.Add(new MqttUser
                {
                    Password = "Test",
                    Username = "Hans"
                });

            var sensors = new[]
            {
                new SensorType
                {
                    Name = "dht11",
                    Created = DateTime.Now
                },
                new SensorType
                {
                    Name = "bmp180",
                    Created = DateTime.Now
                },
                new SensorType
                {
                    Name = "tsic",
                    Created = DateTime.Now
                },
                new SensorType
                {
                    Name = "dht22",
                    Created = DateTime.Now
                },
                new SensorType
                {
                    Name = "dallas",
                    Created = DateTime.Now
                }
            };

            foreach (var sensor in sensors)
            {
                var s = context.SensorTypes.FirstOrDefault(x => x.Name == sensor.Name);
                if (s == null)
                    context.SensorTypes.Add(sensor);
            }

            var rooms = new[]
            {
                new Room
                {
                    Name = "kitchen",
                    Created = DateTime.Now
                },
                new Room
                {
                    Name = "garden",
                    Created = DateTime.Now
                },
                new Room
                {
                    Name = "office",
                    Created = DateTime.Now
                },
                new Room
                {
                    Name = "mobilehome",
                    Created = DateTime.Now
                }
            };

            foreach (var room in rooms)
            {
                var current = context.Rooms.FirstOrDefault(r => r.Name == room.Name);
                if (current == null)
                    context.Rooms.Add(room);
            }

            var measureTypes = new[]
            {
                new MeasureType
                {
                    Name = "temperature",
                    Created = DateTime.Now
                },
                new MeasureType
                {
                    Name = "humidity",
                    Created = DateTime.Now
                },
                new MeasureType
                {
                    Name = "pressure",
                    Created = DateTime.Now
                },
                new MeasureType
                {
                    Name = "altitude",
                    Created = DateTime.Now
                }
            };

            context.SaveChanges();
        }
    }
}