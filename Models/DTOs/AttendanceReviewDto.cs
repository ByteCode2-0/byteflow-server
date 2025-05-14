using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace byteflow_server.Models.DTOs
{
    public class AttendanceReviewDto
    {
        public long ReviewedBy { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttendanceStatus Status { get; set; }
    }
} 