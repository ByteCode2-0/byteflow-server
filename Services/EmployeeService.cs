using byteflow_server.DataAccess;
using byteflow_server.Models;
using byteflow_server.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace byteflow_server.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly ByteFlowDbContext _context;
        public EmployeeService(IGenericRepository<Employee> employeeRepository, ByteFlowDbContext context)
        {
            _employeeRepository = employeeRepository;
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(long id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task CreateEmployeeAsync(Employee employee)
        {
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
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
    }
}
