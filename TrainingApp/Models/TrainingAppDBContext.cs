using System.Data.Entity;
using TrainingApp.ViewModels;

namespace TrainingApp.Models
{
    public class TrainingAppDBContext : DbContext
    {
        public TrainingAppDBContext() : base("TrainingAppConntectionString")
        {
        }
        /*
                public DbSet<Student> Students { get; set; }
                public DbSet<Supervisor> Supervisors { get; set; }
                public DbSet<Admin> Admins { get; set; }

                */
        public DbSet<Users> Users { get; set; }
        public DbSet<Reports> Reports { get; set; }
        public DbSet<MessageViewModel> Messages { get; set; }
    }
}