using System.ComponentModel.DataAnnotations;

namespace byteflow_server.Models.DTOs
{
    public class UserRoleUpdateDto
    {
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
    }
} 