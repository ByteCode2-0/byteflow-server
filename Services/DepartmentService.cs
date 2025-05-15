using byteflow_server.Models;
using byteflow_server.Repositories;
using byteflow_server.Services;
namespace byteflow_server.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IGenericRepository<Department> _departmentRepository;

        public DepartmentService(IGenericRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(long id)
        {
            return await _departmentRepository.GetByIdAsync(id);
        }

        public async Task CreateDepartmentAsync(Department department)
        {
            await _departmentRepository.AddAsync(department);
            await _departmentRepository.SaveChangesAsync();
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            _departmentRepository.Update(department);
            await _departmentRepository.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(long id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department != null)
            {
                _departmentRepository.Delete(department);
                await _departmentRepository.SaveChangesAsync();
            }
        }
    }
}
