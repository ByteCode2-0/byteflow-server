using byteflow_server.DataAccess;
using byteflow_server.Models;
using byteflow_server.Repositories;
using Microsoft.EntityFrameworkCore;

namespace byteflow_server.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly IGenericRepository<LeaveRequest> _leaveRepository;
        private readonly ByteFlowDbContext _context;

        public LeaveService(IGenericRepository<LeaveRequest> leaveRepository, ByteFlowDbContext context)
        {
            _leaveRepository = leaveRepository;
            _context = context;
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync()
        {
            return await _leaveRepository.GetAllAsync();
        }

        public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(long id)
        {
            return await _leaveRepository.GetByIdAsync(id);
        }

        public async Task CreateLeaveRequestAsync(LeaveRequest leaveRequest)
        {
            await _leaveRepository.AddAsync(leaveRequest);
            await _leaveRepository.SaveChangesAsync();
        }

        public async Task UpdateLeaveRequestAsync(LeaveRequest leaveRequest)
        {
            var existingLeaveRequest = await _leaveRepository.GetByIdAsync(leaveRequest.LeaveRequestId);
            if (existingLeaveRequest != null)
            {
                _context.Entry(existingLeaveRequest).State = EntityState.Detached;
                _leaveRepository.Update(leaveRequest);
                await _leaveRepository.SaveChangesAsync();
            }
        }

        public async Task DeleteLeaveRequestAsync(long id)
        {
            var leaveRequest = await _leaveRepository.GetByIdAsync(id);
            if (leaveRequest != null)
            {
                _leaveRepository.Delete(leaveRequest);
                await _leaveRepository.SaveChangesAsync();
            }
        }
    }
}
