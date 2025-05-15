using byteflow_server.DataAccess;
using byteflow_server.Models;
using byteflow_server.Models.DTOs;
using byteflow_server.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace byteflow_server.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly ByteFlowDbContext _context;

        public EmployeeService(
            IGenericRepository<Employee> employeeRepository,
            IGenericRepository<User> userRepository,
            ByteFlowDbContext context)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.User)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(long id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message, Employee? Employee)> CreateEmployeeWithUserAsync(EmployeeCreateDto createDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if username or email already exists
                if (await _context.Users.AnyAsync(u => u.UserName == createDto.UserName || u.Email == createDto.Email))
                {
                    return (false, "Username or email already exists", null);
                }

                // Create User
                var user = new User
                {
                    UserName = createDto.UserName,
                    Email = createDto.Email,
                    Password = createDto.Password,
                    Role = "Employee", // Default role for new employees
                    IsActive = true
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                // Create Employee
                var employee = new Employee
                {
                    UserId = user.UserId,
                    EmployeeName = createDto.EmployeeName,
                    PhoneNumber = createDto.PhoneNumber,
                    DepartmentId = createDto.DepartmentId,
                    Address = createDto.Address,
                    DateOfBirth = createDto.DateOfBirth,
                    PhotoUrl = createDto.PhotoUrl,
                    JoinDate = DateTime.UtcNow
                };

                await _employeeRepository.AddAsync(employee);
                await _employeeRepository.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, "Employee created successfully", employee);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Error creating employee: {ex.Message}", null);
            }
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            employee.UpdatedAt = DateTime.UtcNow;
            var existingEmployee = await _employeeRepository.GetByIdAsync(employee.EmployeeId);
            if (existingEmployee != null)
            {
                _context.Entry(existingEmployee).State = EntityState.Detached;
                _employeeRepository.Update(employee);
                await _employeeRepository.SaveChangesAsync();
            }
        }

        public async Task DeleteEmployeeAsync(long id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee != null)
            {
                _employeeRepository.Delete(employee);
                await _employeeRepository.SaveChangesAsync();
            }
        }

        public async Task<(bool Success, string Message, Employee? Employee)> PatchUpdateUserEmployeeAsync(long employeeId, UserEmployeeUpdateDto updateDto)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return (false, "Employee not found", null);
            }

            var user = await _userRepository.GetByIdAsync(employee.UserId);
            if (user == null)
            {
                return (false, "User not found", null);
            }

            // Update user fields if provided
            if (!string.IsNullOrEmpty(updateDto.UserName)) user.UserName = updateDto.UserName;
            if (!string.IsNullOrEmpty(updateDto.Email)) user.Email = updateDto.Email;
            if (!string.IsNullOrEmpty(updateDto.Password)) user.Password = updateDto.Password;

            // Update employee fields if provided
            if (!string.IsNullOrEmpty(updateDto.EmployeeName)) employee.EmployeeName = updateDto.EmployeeName;
            if (updateDto.PhoneNumber.HasValue) employee.PhoneNumber = updateDto.PhoneNumber.Value;
            if (updateDto.DepartmentId.HasValue) employee.DepartmentId = updateDto.DepartmentId.Value;
            if (!string.IsNullOrEmpty(updateDto.Address)) employee.Address = updateDto.Address;
            if (updateDto.DateOfBirth.HasValue) employee.DateOfBirth = updateDto.DateOfBirth.Value;
            if (!string.IsNullOrEmpty(updateDto.PhotoUrl)) employee.PhotoUrl = updateDto.PhotoUrl;

            employee.UpdatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            _employeeRepository.Update(employee);
            await _userRepository.SaveChangesAsync();
            await _employeeRepository.SaveChangesAsync();

            return (true, "User and employee updated successfully", employee);
        }
    }
}
