using byteflow_server.Models;
using byteflow_server.Repositories;

namespace byteflow_server.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IGenericRepository<Attendance> _attendanceRepository;

        public AttendanceService(IGenericRepository<Attendance> attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync()
        {
            return await _attendanceRepository.GetAllAsync();
        }

        public async Task<Attendance?> GetAttendanceByIdAsync(long id)
        {
            return await _attendanceRepository.GetByIdAsync(id);
        }

        public async Task CreateAttendanceAsync(Attendance attendance)
        {
            await _attendanceRepository.AddAsync(attendance);
            await _attendanceRepository.SaveChangesAsync();
        }

        public async Task UpdateAttendanceAsync(Attendance attendance)
        {
            var existingAttendance = await _attendanceRepository.GetByIdAsync(attendance.AttendanceId);
            if (existingAttendance != null)
            {
                _attendanceRepository.Update(attendance);
                await _attendanceRepository.SaveChangesAsync();
            }
        }

        public async Task DeleteAttendanceAsync(long id)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(id);
            if (attendance != null)
            {
                _attendanceRepository.Delete(attendance);
                await _attendanceRepository.SaveChangesAsync();
            }
        }
    }
}
