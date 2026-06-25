using Microsoft.EntityFrameworkCore;
using WebProjekat.Application.DTOs;
using WebProjekat.Application.Interfaces;
using WebProjekat.Infrastructure.Persistence;

namespace WebProjekat.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
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
            var user = await _context.Users
                .Include(u => u.TravelPlans)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AdminTravelPlanDTO>> GetAllTravelPlansAsync()
        {
            return await _context.TravelPlans
                .Include(p => p.Owner)
                .Select(p => new AdminTravelPlanDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    PlannedBudget = p.PlannedBudget,
                    OwnerName = p.Owner.Name,
                    OwnerEmail = p.Owner.Email,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<AdminTravelPlanDetailDTO> GetTravelPlanAsync(Guid planId)
        {
            return await _context.TravelPlans
                .Include(p => p.Owner)
                .Include(p => p.Destinations)
                .Include(p => p.Activities)
                .Include(p => p.Expenses)
                .Include(p => p.ChecklistItems)
                .Where(p => p.Id == planId)
                .Select(p => new AdminTravelPlanDetailDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    PlannedBudget = p.PlannedBudget,
                    OwnerName = p.Owner.Name,
                    OwnerEmail = p.Owner.Email,
                    CreatedAt = p.CreatedAt,
                    TotalExpenses = p.Expenses.Sum(e => e.Amount),
                    DestinationCount = p.Destinations.Count,
                    ActivityCount = p.Activities.Count,
                    ExpenseCount = p.Expenses.Count,
                    ChecklistItemCount = p.ChecklistItems.Count
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteTravelPlanAsync(Guid planId)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId);
            if (plan == null) return false;

            _context.TravelPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AdminStatisticsDTO> GetStatisticsAsync()
        {
            return new AdminStatisticsDTO
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalPlans = await _context.TravelPlans.CountAsync(),
                TotalDestinations = await _context.Destinations.CountAsync(),
                TotalActivities = await _context.Activities.CountAsync(),
                TotalExpenses = await _context.Expenses.SumAsync(e => e.Amount)
            };
        }
    }
}