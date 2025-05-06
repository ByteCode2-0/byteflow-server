using System.ComponentModel.DataAnnotations;

namespace byteflow_server.Models.DTOs
{
    public class EmployeeCreateDto
    {
        // User Information
        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        // Employee Information
        [Required(ErrorMessage = "Employee name is required")]
        public string? EmployeeName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public long PhoneNumber { get; set; }

        [Required(ErrorMessage = "Department ID is required")]
        public long DepartmentId { get; set; }

        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhotoUrl { get; set; }
    }
} 