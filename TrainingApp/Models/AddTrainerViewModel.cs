using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TrainingApp.Models;

namespace TrainingApp.ViewModels
{
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

        public IEnumerable<Users> UniversitySupervisors { get; set; }
    }
}
