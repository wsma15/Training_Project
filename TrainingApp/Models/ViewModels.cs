using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;
using TrainingApp.Models;

namespace TrainingApp.ViewModels
{
    public class AddCompanySupervisorViewModel
    {
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class AddSupervisorViewModel
    {
        [Required(ErrorMessage = "Supervisor Name is required")]
        public string SupervisorName { get; set; }

        [Required(ErrorMessage = "Supervisor Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string SupervisorEmail { get; set; }

        [Required(ErrorMessage = "Supervisor Password is required")]
        [DataType(DataType.Password)]
        public string SupervisorPassword { get; set; }

        [Required(ErrorMessage = "University Name is required")]
        public string UniversityName { get; set; }
    }

    public class AddTrainerViewModel
    {
        [Required]
        [StringLength(50)]
        public string TrainerName { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string TrainerEmail { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string TrainerPassword { get; set; }

        [Required]
        public int UniversitySupervisorID { get; set; }

        [Required]
        public int CompanySupervisorID { get; set; }

        public IEnumerable<SelectListItem> UniversitySupervisors { get; set; }
        public IEnumerable<SelectListItem> CompanySupervisors { get; set; }

        public string CompanyName { get; set; } // To hold the selected company's name
    }
    public class DashboardViewModel
    {
        public List<Users> Trainers { get; set; }
        public List<Users> UniversitySupervisors { get; set; }
        public List<Users> CompanySupervisors { get; set; }
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();

        public string GetUniName(int supervisorId)
        {
            // Assuming you have a method to retrieve supervisor details from the database
            string supervisor = (from name in _context.Users where name.Roles == UserRole.UniversitySupervisor && name.Id == supervisorId select name.UniversityName).FirstOrDefault();

            if (supervisor != null)
            {
                return supervisor;
            }
            else
            {
                return "University not found"; // Or handle the case when supervisor is not found
            }
        }
        public string GetCompanyName(int supervisorId)
        {
            // Assuming you have a method to retrieve supervisor details from the database
            string supervisor = (from name in _context.Users where name.Roles == UserRole.CompanySupervisor && name.Id == supervisorId select name.CompanyName).FirstOrDefault();

            if (supervisor != null)
            {
                return supervisor;
            }
            else
            {
                return "Company not found"; // Or handle the case when supervisor is not found
            }
        }
        public string GetSupervisorName(int supervisorId)
        {
            // Assuming you have a method to retrieve supervisor details from the database
            var supervisor = _context.Users.FirstOrDefault(s => s.Id == supervisorId);

            if (supervisor != null)
            {
                return supervisor.Name;
            }
            else
            {
                return "Supervisor not found"; // Or handle the case when supervisor is not found
            }
        }
    }
    public class ChatMessageViewModel
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class CombinedRegistrationViewModel
    {
        public AddTrainerViewModel StudentViewModel { get; set; }
        public AddCompanySupervisorViewModel CompanySupervisorViewModel { get; set; }
        public AddSupervisorViewModel UniversitySupervisorViewModel { get; set; }
        public IEnumerable<SelectListItem> UniversitySupervisors { get; set; }
        public IEnumerable<SelectListItem> CompanySupervisors { get; set; }
    }
    public class CompanySupervisorDashboardViewModel
    {
        public string CompanyName { get; set; }
        public int UserCount { get; set; }
    }

    public class MessageViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string SenderName { get; set; }

        [Required]
        public string ReceiverName { get; set; }

        [Required]
        public string MessageText { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
        public List<ChatMessageViewModel> ChatHistory { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }

    }

}