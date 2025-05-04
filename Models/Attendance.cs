using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace byteflow_server.Models
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AttendanceId { get; set; }

        public long AttendeeId { get; set; }

        [ForeignKey("AttendeeId")]
        public Employee? Attendee { get; set; }

        public long? ReviewedBy { get; set; }

        [ForeignKey("ReviewedBy")]
        public Employee? Reviewer { get; set; }

        public DateTime? CheckInTime { get; set; } = DateTime.UtcNow;
        public DateTime? CheckOutTime { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool? IsDeleted { get; set; } = false;
        public bool? Status { get; set; } = true;
    }
}
