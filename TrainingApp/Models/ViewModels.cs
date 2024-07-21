using Microsoft.Ajax.Utilities;
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
        [Required(ErrorMessage = "Full Name is Required")]
        public string FullName { get; set; }
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
    [Required(ErrorMessage = "University Name is required")]
    public string UniversityName { get; set; }

    [Required(ErrorMessage = "Supervisor Name is required")]
    public string SupervisorName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string SupervisorEmail { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    public string SupervisorPassword { get; set; }
}

    public class AddTrainerViewModel
    {
        [Required(ErrorMessage = "Trainer Name is required.")]
        [StringLength(50, ErrorMessage = "Trainer Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Trainer Name can only contain letters and spaces.")]
        public string TrainerName { get; set; }

        [Required(ErrorMessage = "Trainer Email is required.")]
        [StringLength(50, ErrorMessage = "Trainer Email cannot exceed 50 characters.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string TrainerEmail { get; set; }

        [Required(ErrorMessage = "Trainer Password is required.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$", ErrorMessage = "Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string TrainerPassword { get; set; }

        [Required(ErrorMessage = "University Supervisor is required.")]
        public int UniversitySupervisorID { get; set; }

        [Required(ErrorMessage = "Company Supervisor is required.")]
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
        public List<Users> NewUsers { get; set; }

        public AddSupervisorViewModel addSupervisorViewModel { get; set; }
        public AddCompanySupervisorViewModel addCompanySupervisorViewModel { get; set; }
        public AddTrainerViewModel addTrainerViewModel { get; set; }

        private readonly TrainingAppDBContext _context;

        public DashboardViewModel()
        {
            _context = new TrainingAppDBContext();
        }

        public string GetUniName(int supervisorId)
        {
            string supervisor = (from name in _context.Users
                                 where name.Roles == UserRole.UniversitySupervisor && name.Id == supervisorId
                                 select name.UniversityName).FirstOrDefault();

            return supervisor ?? "University not found";
        }

        public string GetCompanyName(int supervisorId)
        {
            string supervisor = (from name in _context.Users
                                 where name.Roles == UserRole.CompanySupervisor && name.Id == supervisorId
                                 select name.CompanyName).FirstOrDefault();

            return supervisor ?? "Company not found";
        }

        public string GetSupervisorName(int supervisorId)
        {
            var supervisor = _context.Users.FirstOrDefault(s => s.Id == supervisorId);

            return supervisor?.Name ?? "Supervisor not found";
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