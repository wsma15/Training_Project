using System.ComponentModel.DataAnnotations;

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
}
