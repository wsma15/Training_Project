using System.ComponentModel.DataAnnotations;

namespace TrainingApp.ViewModels
{
    public class AddSupervisorViewModel
    {
        [Required]
        public string SupervisorName { get; set; }

        [Required]
        public string SupervisorEmail { get; set; }

        [Required]
        public string SupervisorPassword { get; set; }
    }
}
