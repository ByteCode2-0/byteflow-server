using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace byteflow_server.Models
{
    public enum LeaveStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class LeaveRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LeaveRequestId { get; set; }

        [Required]
        public long EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? LeaveTaker { get; set; }

        [Required]
        public long? ReviewedBy { get; set; }

        [ForeignKey("ReviewedBy")]
        public User? Reviewer { get; set; }

        public string? LeaveType { get; set; }
        public string? FeedBack { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Reason { get; set; }
        public DateTime? AppliedDate { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool? IsDeleted { get; set; } = false;
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
       
    }
}
