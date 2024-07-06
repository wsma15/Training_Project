using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Key]
        public string StudentID { get; set; }

        [Required]
        public string StudentPassword { get; set; }

        [Required]
        public string StudentName { get; set; }

        [Required]
        public string StudentEmail { get; set; }

        public string SupervisorID { get; set; }

        [ForeignKey("SupervisorID")]
        public virtual Supervisor Supervisor { get; set; }
    }
}
