using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebProjekat.Application.Interfaces;

namespace Web1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        private bool IsAdmin()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return role == "Admin" || role == "Administrator";
        }

        [HttpGet("users")]
        [Tags("Admin - Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!IsAdmin()) return Forbid();
            return Ok(await _adminService.GetAllUsersAsync());
        }

        [HttpGet("users/{userId:guid}")]
        [Tags("Admin - Users")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            if (!IsAdmin()) return Forbid();
            var user = await _adminService.GetUserAsync(userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpDelete("users/{userId:guid}")]
        [Tags("Admin - Users")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            if (!IsAdmin()) return Forbid();
            var ok = await _adminService.DeleteUserAsync(userId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("travelplans")]
        [Tags("Admin - Travel Plans")]
        public async Task<IActionResult> GetAllTravelPlans()
        {
            if (!IsAdmin()) return Forbid();
            return Ok(await _adminService.GetAllTravelPlansAsync());
        }

        [HttpGet("travelplans/{planId:guid}")]
        [Tags("Admin - Travel Plans")]
        public async Task<IActionResult> GetTravelPlan(Guid planId)
        {
            if (!IsAdmin()) return Forbid();
            var plan = await _adminService.GetTravelPlanAsync(planId);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpDelete("travelplans/{planId:guid}")]
        [Tags("Admin - Travel Plans")]
        public async Task<IActionResult> DeleteTravelPlan(Guid planId)
        {
            if (!IsAdmin()) return Forbid();
            var ok = await _adminService.DeleteTravelPlanAsync(planId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("statistics")]
        [Tags("Admin - Statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            if (!IsAdmin()) return Forbid();
            return Ok(await _adminService.GetStatisticsAsync());
        }
    }
}