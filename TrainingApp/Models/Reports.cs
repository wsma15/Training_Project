using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Reports
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string ReportId { get; set; }

        [Required]
        public string ReportName { get; set; }

        [Required]
        public string ReportTitle { get; set; }

        public string ReportDescription { get; set; }

        [Required]
        public DateTime ReportDateTime { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        public string SupervisorID { get; set; }
    }
}
