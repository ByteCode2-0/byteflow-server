using byteflow_server.Models;
using byteflow_server.Models.DTOs;

namespace byteflow_server.Services
{
    public interface IAttendanceService
    {
        Task<IEnumerable<Attendance>> GetAllAttendancesAsync();
        Task<Attendance?> GetAttendanceByIdAsync(long id);
        Task<(bool Success, string Message, Attendance? Attendance)> CreateAttendanceAsync(Attendance attendance);
        Task<(bool Success, string Message)> ReviewAttendanceAsync(long attendanceId, AttendanceReviewDto reviewDto);
        Task UpdateAttendanceAsync(Attendance attendance);
        Task DeleteAttendanceAsync(long id);
        Task<IEnumerable<Attendance>> GetAttendancesByAttendeeIdAsync(long attendeeId);
    }
}
