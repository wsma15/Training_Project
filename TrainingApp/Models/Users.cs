using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Users
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string UniversitySupervisorID { get; set; }

        public string UniversityName { get; set; }

        public string CompanyName { get; set; }

        [Required]
        public UserRole Roles { get; set; }
    }

    public enum UserRole
    {
        Admin,
        UniversitySupervisor,
        Trainer,
        CompanySupervisor
    }
}