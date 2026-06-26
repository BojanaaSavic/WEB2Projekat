using Microsoft.EntityFrameworkCore;
using WebProjekat.Application.DTOs;
using WebProjekat.Application.Interfaces;
using WebProjekat.Infrastructure.Persistence;

namespace WebProjekat.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserDbContext _context;

        public AdminService(UserDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminUserDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new AdminUserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();
        }

        public async Task<AdminUserDTO> GetUserAsync(Guid userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new AdminUserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AdminStatisticsDTO> GetUserStatisticsAsync()
        {
            return new AdminStatisticsDTO
            {
                TotalUsers = await _context.Users.CountAsync()
            };
        }
    }
}