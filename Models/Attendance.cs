using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace byteflow_server.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AttendanceStatus
    {
        Pending,
        Present,
        Absent,
        Late
    }

    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AttendanceId { get; set; }

        [Required]
        public long AttendeeId { get; set; }

        public long? ReviewedBy { get; set; }

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Pending;
    }
}
