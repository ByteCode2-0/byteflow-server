﻿using byteflow_server.Models;
using byteflow_server.Models.DTOs;

namespace byteflow_server.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<IEnumerable<Employee>> GetReviewersAsync();
        Task<Employee?> GetEmployeeByIdAsync(long id);
        Task<(bool Success, string Message, Employee? Employee)> CreateEmployeeWithUserAsync(EmployeeCreateDto createDto);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(long id);
        Task<(bool Success, string Message, Employee? Employee)> PatchUpdateUserEmployeeAsync(long employeeId, UserEmployeeUpdateDto updateDto);
    }
}
