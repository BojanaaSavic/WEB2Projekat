using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebProjekat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Web1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return role == "Admin" || role == "Administrator";
        }

        [HttpGet("users")]
        [Tags("Admin - Users")]
        /// <summary>
        /// List all users in the system (Admin only)
        /// </summary>
        public async Task<IActionResult> GetAllUsers()
        {
            if (!IsAdmin()) return Forbid();

            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("users/{userId:guid}")]
        [Tags("Admin - Users")]
        /// <summary>
        /// Get details of a specific user (Admin only)
        /// </summary>
        public async Task<IActionResult> GetUser(Guid userId)
        {
            if (!IsAdmin()) return Forbid();

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Role
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpDelete("users/{userId:guid}")]
        [Tags("Admin - Users")]
        /// <summary>
        /// Delete a user account (Admin only)
        /// </summary>
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            if (!IsAdmin()) return Forbid();

            var user = await _context.Users
                .Include(u => u.TravelPlans)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("travelplans")]
        [Tags("Admin - Travel Plans")]
        /// <summary>
        /// List all travel plans in the system (Admin only)
        /// </summary>
        public async Task<IActionResult> GetAllTravelPlans()
        {
            if (!IsAdmin()) return Forbid();

            var plans = await _context.TravelPlans
                .Include(p => p.Owner)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.StartDate,
                    p.EndDate,
                    p.PlannedBudget,
                    OwnerName = p.Owner.Name,
                    OwnerEmail = p.Owner.Email,
                    p.CreatedAt
                })
                .ToListAsync();

            return Ok(plans);
        }

        [HttpGet("travelplans/{planId:guid}")]
        [Tags("Admin - Travel Plans")]
        /// <summary>
        /// Get details of a specific travel plan with statistics (Admin only)
        /// </summary>
        public async Task<IActionResult> GetTravelPlan(Guid planId)
        {
            if (!IsAdmin()) return Forbid();

            var plan = await _context.TravelPlans
                .Include(p => p.Owner)
                .Include(p => p.Destinations)
                .Include(p => p.Activities)
                .Include(p => p.Expenses)
                .Include(p => p.ChecklistItems)
                .Where(p => p.Id == planId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.StartDate,
                    p.EndDate,
                    p.PlannedBudget,
                    OwnerName = p.Owner.Name,
                    OwnerEmail = p.Owner.Email,
                    TotalExpenses = p.Expenses.Sum(e => e.Amount),
                    DestinationCount = p.Destinations.Count,
                    ActivityCount = p.Activities.Count,
                    ExpenseCount = p.Expenses.Count,
                    ChecklistItemCount = p.ChecklistItems.Count,
                    p.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpDelete("travelplans/{planId:guid}")]
        [Tags("Admin - Travel Plans")]
        /// <summary>
        /// Delete a travel plan (Admin only)
        /// </summary>
        public async Task<IActionResult> DeleteTravelPlan(Guid planId)
        {
            if (!IsAdmin()) return Forbid();

            var plan = await _context.TravelPlans.FirstOrDefaultAsync(p => p.Id == planId);
            if (plan == null) return NotFound();

            _context.TravelPlans.Remove(plan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("statistics")]
        [Tags("Admin - Statistics")]
        /// <summary>
        /// Get system-wide statistics (Admin only)
        /// </summary>
        public async Task<IActionResult> GetStatistics()
        {
            if (!IsAdmin()) return Forbid();

            var totalUsers = await _context.Users.CountAsync();
            var totalPlans = await _context.TravelPlans.CountAsync();
            var totalDestinations = await _context.Destinations.CountAsync();
            var totalActivities = await _context.Activities.CountAsync();
            var totalExpenses = await _context.Expenses.SumAsync(e => e.Amount);

            return Ok(new
            {
                totalUsers,
                totalPlans,
                totalDestinations,
                totalActivities,
                totalExpenses
            });
        }
    }
}
