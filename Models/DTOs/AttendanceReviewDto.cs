using System.ComponentModel.DataAnnotations;

namespace byteflow_server.Models.DTOs
{
    public class AttendanceReviewDto
    {
        [Required(ErrorMessage = "Reviewer ID is required")]
        public long ReviewerId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public AttendanceStatus Status { get; set; }
    }
} 