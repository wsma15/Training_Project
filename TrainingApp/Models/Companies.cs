using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrainingApp.Models
{
    public class Companies
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string City { get; set; }

    }
}