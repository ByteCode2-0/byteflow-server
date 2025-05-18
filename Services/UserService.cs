using byteflow_server.DataAccess;
using byteflow_server.Models;
using byteflow_server.Models.DTOs;
using byteflow_server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace byteflow_server.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly DbContext _context;

        public UserService(IGenericRepository<User> userRepository, ByteFlowDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(long id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task CreateUserAsync(User user)
        {
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.UserId);
            if (existingUser != null)
            {
                _context.Entry(existingUser).State = EntityState.Detached;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
            }

          
        }

        public async Task DeleteUserAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                _userRepository.Delete(user);
                await _userRepository.SaveChangesAsync();
            }
        }

        public async Task<(bool Success, string Message)> UpdateUserRoleAsync(long userId, UserRoleUpdateDto roleUpdateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            user.Role = roleUpdateDto.Role;
            user.UpdatedAt = DateTime.UtcNow;
            
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            
            return (true, "User role updated successfully");
        }
    }

}
