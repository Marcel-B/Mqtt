using System.Collections.Generic;
using com.b_velop.Mqtt.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace com.b_velop.Mqtt.Context
{
    public class DataContext : DbContext
    {
        public DbSet<MqttUser> MqttUsers { get; set; }
        public DbSet<MeasureTime> MeasureTimes { get; set; }
        public DbSet<MeasureValue> MeasureValues { get; set; }
        public DbSet<MqttMessage> MqttMessages { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<SensorType> SensorTypes { get; set; }

        public DataContext(DbContextOptions<DataContext> options) :base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MqttUser>().HasKey(user => user.Username);
            modelBuilder.Entity<MeasureTime>().HasKey(time => time.Timestamp);
            modelBuilder.Entity<MeasureType>().HasKey(type => type.Name);
            modelBuilder.Entity<Room>().HasKey(k => k.Name);
            modelBuilder.Entity<SensorType>().HasKey(k => k.Name);
            
            modelBuilder.Entity<MeasureValue>().HasKey(value =>
                new {value.RoomName, value.MeasureTimeTimestamp, value.SensorTypeName, value.MeasureTypeName});
        }
    }
}