﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Users
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public int? UniversitySupervisorID { get; set; }
        public int? CompanySupervisorID { get; set; }

        public int UniversityID { get; set; }

        public int CompanyID { get; set; }

        public UserRole Roles { get; set; }
        public DateTime LastLogin { get; set; } 
        public string ProfilePicturePath { get; set; }
    }

    public enum UserRole
    {
        Admin,
        UniversitySupervisor,
        Trainer,
        CompanySupervisor,
        NewUser
    }
}