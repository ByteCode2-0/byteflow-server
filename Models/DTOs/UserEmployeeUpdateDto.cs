using System;
using System.ComponentModel.DataAnnotations;

namespace byteflow_server.Models.DTOs
{
    public class UserEmployeeUpdateDto
    {
        // User Information
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }

        // Employee Information
        public string? EmployeeName { get; set; }
        public long? PhoneNumber { get; set; }
        public long? DepartmentId { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhotoUrl { get; set; }
    }
} 