using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace byteflow_server.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeId { get; set; }

        [Required]
        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public string? EmployeeName { get; set; }

        [Required]
        public long PhoneNumber { get; set; }

        [Required]
        public long DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        public string? Address { get; set; }
        public DateTime? JoinDate { get; set; } = DateTime.UtcNow;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DateOfBirth { get; set; }
        public string? PhotoUrl { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public bool? Status { get; set; } = true;
    }
}
