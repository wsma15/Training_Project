using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Reports
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int ReportId { get; set; }

        [Required(ErrorMessage = "Report Title is required")]
        public string ReportTitle { get; set; }

        [Required(ErrorMessage = "Report Description is required")]
        public string ReportDescription { get; set; }

        [Required]
        public DateTime ReportDateCreated { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        public string SupervisorID { get; set; }
    }
}
