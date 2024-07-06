using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Admin
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Key]
        public string AdminId { get; set; }
        [Required]

        public string AdminName { get; set; }

        [Required]
        public string AdminPassword { get; set; }

    }
}