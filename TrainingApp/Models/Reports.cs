using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Reports
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        [Required]
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
        [Required(ErrorMessage = "Report File is required")]
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }

        public bool IsApproved { get; set; }
        public string Feedback { get; set; }
        public bool IsFeedbackSubmitted { get; set; } // Indicates whether feedback has been submitted

    }
}
