using byteflow_server.DataAccess;
using byteflow_server.Models;
using byteflow_server.Models.DTOs;
using byteflow_server.Repositories;
using Microsoft.EntityFrameworkCore;

namespace byteflow_server.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IGenericRepository<Attendance> _attendanceRepository;
        private readonly ByteFlowDbContext _context;

        public AttendanceService(IGenericRepository<Attendance> attendanceRepository, ByteFlowDbContext context)
        {
            _attendanceRepository = attendanceRepository;
            _context = context;
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync()
        {
            return await _attendanceRepository.GetAllAsync();
        }

        public async Task<Attendance?> GetAttendanceByIdAsync(long id)
        {
            return await _attendanceRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message, Attendance? Attendance)> CreateAttendanceAsync(Attendance attendance)
        {
            try
            {
                // Validate required fields
                if (attendance.AttendeeId <= 0)
                {
                    return (false, "AttendeeId is required and must be greater than 0", null);
                }

                if (!attendance.CheckInTime.HasValue)
                {
                    return (false, "CheckInTime is required", null);
                }

                // Check if attendance already exists for this employee on this check-in time
                var existingAttendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.AttendeeId == attendance.AttendeeId && 
                                            a.CheckInTime.HasValue &&
                                            a.CheckInTime.Value.Date == attendance.CheckInTime.Value.Date && 
                                            !a.IsDeleted);

                if (existingAttendance != null)
                {
                    return (false, "Attendance record already exists for this employee on this date", null);
                }

                // Set default values
                attendance.CreatedAt = DateTime.UtcNow;
                attendance.UpdatedAt = DateTime.UtcNow;
                attendance.IsDeleted = false;
                attendance.Status = AttendanceStatus.Pending; // Ensure status is Pending for new records
                attendance.ReviewedBy = null; // Ensure ReviewedBy is null for new records
                attendance.CheckOutTime = null; // Ensure CheckOutTime is null for new records

                await _attendanceRepository.AddAsync(attendance);
                await _attendanceRepository.SaveChangesAsync();

                return (true, "Attendance record created successfully", attendance);
            }
            catch (Exception ex)
            {
                return (false, $"Error creating attendance record: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> ReviewAttendanceAsync(long attendanceId, AttendanceReviewDto reviewDto)
        {
            try
            {
                var attendance = await _attendanceRepository.GetByIdAsync(attendanceId);
                if (attendance == null)
                {
                    return (false, "Attendance record not found");
                }

                if (attendance.Status != AttendanceStatus.Pending)
                {
                    return (false, "This attendance record has already been reviewed");
                }

                attendance.ReviewedBy = reviewDto.ReviewerId;
                attendance.Status = reviewDto.Status;
                attendance.UpdatedAt = DateTime.UtcNow;

                _attendanceRepository.Update(attendance);
                await _attendanceRepository.SaveChangesAsync();

                return (true, "Attendance record reviewed successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error reviewing attendance record: {ex.Message}");
            }
        }

        public async Task UpdateAttendanceAsync(Attendance attendance)
        {
            var existingAttendance = await _attendanceRepository.GetByIdAsync(attendance.AttendanceId);
            if (existingAttendance != null)
            {
                _context.Entry(existingAttendance).State = EntityState.Detached;
                attendance.UpdatedAt = DateTime.UtcNow;
                _attendanceRepository.Update(attendance);
                await _attendanceRepository.SaveChangesAsync();
            }
        }

        public async Task DeleteAttendanceAsync(long id)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(id);
            if (attendance != null)
            {
                attendance.IsDeleted = true;
                attendance.UpdatedAt = DateTime.UtcNow;
                _attendanceRepository.Update(attendance);
                await _attendanceRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Attendance>> GetAttendancesByAttendeeIdAsync(long attendeeId)
        {
            return await _context.Attendances
                .Where(a => a.AttendeeId == attendeeId && !a.IsDeleted)
                .OrderByDescending(a => a.CheckInTime)
                .ToListAsync();
        }
    }
}
