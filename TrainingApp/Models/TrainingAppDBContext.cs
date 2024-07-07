﻿using System.Data.Entity;

namespace TrainingApp.Models
{
    public class TrainingAppDBContext : DbContext
    {
        public TrainingAppDBContext() : base("TrainingAppConntectionString")
        {

        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Reports> Reports { get; set; }

    }
}