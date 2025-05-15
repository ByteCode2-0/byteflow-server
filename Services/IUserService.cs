using byteflow_server.Models;
using byteflow_server.Models.DTOs;

namespace byteflow_server.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(long id);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(long id);
        Task<(bool Success, string Message)> UpdateUserRoleAsync(long userId, UserRoleUpdateDto roleUpdateDto);
    }
}
