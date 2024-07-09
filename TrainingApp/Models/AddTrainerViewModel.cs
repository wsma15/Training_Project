using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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

        [Required]
        public int CompanySupervisorID { get; set; }

        public IEnumerable<SelectListItem> UniversitySupervisors { get; set; }
        public IEnumerable<SelectListItem> CompanySupervisors { get; set; }

        public string CompanyName { get; set; } // To hold the selected company's name
    }
}
