using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Supervisor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Key]
        public string SupervisorID { get; set; }

        [Required]
        public string SupervisorName { get; set; }

        [Required]
        public string SupervisorPassword { get; set; }

        [Required]
        public string SupervisorEmail { get; set; }

        public virtual ICollection<Student> Students { get; set; }
    }
}
