using System.ComponentModel.DataAnnotations;

namespace TrainingApp.ViewModels
{
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
}
