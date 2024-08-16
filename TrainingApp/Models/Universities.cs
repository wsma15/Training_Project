using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Universities
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Key]
        public int Id { get; set; }


        [Required]
        public string UniversityName { get; set; }

        [Required]
        public string City { get; set; }

    }
}