using byteflow_server.Models;

namespace byteflow_server.Services
{
    public interface ILeaveService
    {
        Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync();
        Task<LeaveRequest?> GetLeaveRequestByIdAsync(long id);
        Task CreateLeaveRequestAsync(LeaveRequest leaveRequest);
        Task UpdateLeaveRequestAsync(LeaveRequest leaveRequest);
        Task DeleteLeaveRequestAsync(long id);

        Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsReviewByAsync(long id);

        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(long id);
    }
}
