using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TrainingApp.Models;

namespace TrainingApp.ViewModels
{
    public class AddStudentViewModel
    {
        [Required]
        public string StudentName { get; set; }

        [Required]
        public string StudentEmail { get; set; }

        [Required]
        public string StudentPassword { get; set; }

        [Required]
        public string SupervisorID { get; set; }

        public IEnumerable<Supervisor> Supervisors { get; set; }
    }
}
