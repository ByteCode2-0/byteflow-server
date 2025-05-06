using System.ComponentModel.DataAnnotations;

namespace byteflow_server.Models.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username or Email is required")]
        public string? UserNameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
} 