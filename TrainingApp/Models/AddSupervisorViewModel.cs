using System.ComponentModel.DataAnnotations;

namespace TrainingApp.ViewModels
{
    public class AddSupervisorViewModel
    {
        [Required]
        public string SupervisorName { get; set; }

        [Required]
        [EmailAddress]
        public string SupervisorEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string SupervisorPassword { get; set; }
    }
}
