using System.Data.Entity;

namespace TrainingApp.Models
{
    public class TrainingAppDBContext : DbContext
    {
        public TrainingAppDBContext() : base("TrainingAppConnectionString")
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Reports> Reports { get; set; }
        public DbSet<Message> ChatMessages { get; set; }
        public DbSet<Universities> Universities { get; set; }
        public DbSet<Companies> Companies { get; set; }
    }
}