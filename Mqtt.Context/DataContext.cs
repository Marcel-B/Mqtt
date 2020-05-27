using com.b_velop.Mqtt.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace com.b_velop.Mqtt.Context
{
    public class DataContext : DbContext
    {
        public DbSet<MqttUser> MqttUsers { get; set; }

        public DataContext(DbContextOptions<DataContext> options) :base(options)
        {
            
        }
    }
}