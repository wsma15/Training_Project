using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TrainingApp.Models;

namespace TrainingApp.ViewModels
{
    public class AddStudentViewModel
    {
        [Required]
        [StringLength(50)]
        public string StudentName { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string StudentEmail { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string StudentPassword { get; set; }

        [Required]
        public int SupervisorID { get; set; }

        public IEnumerable<Users> Supervisors { get; set; }
    }
}
