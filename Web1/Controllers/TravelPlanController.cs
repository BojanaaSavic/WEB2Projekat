using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WebProjekat.Application.DTOs;
using WebProjekat.Application.Interfaces;
using WebProjekat.Domain.Entities;

namespace Web1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TravelPlansController : ControllerBase
    {
        private readonly ITravelPlanService _travelPlanService;

        public TravelPlansController(ITravelPlanService travelPlanService)
        {
            _travelPlanService = travelPlanService;
        }

        private Guid GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
        }

        [HttpPost]
        [Tags("Travel Plans")]
        /// <summary>
        /// Create a new travel plan
        /// </summary>
        /// <param name="dto">Travel plan details</param>
        public async Task<IActionResult> Create([FromBody] CreateTravelPlanDTO dto)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            try
            {
                var created = await _travelPlanService.CreateAsync(dto, ownerId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Tags("Travel Plans")]
        /// <summary>
        /// Get all travel plans for current user
        /// </summary>
        public async Task<IActionResult> GetAll()
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var plans = await _travelPlanService.GetAllForUserAsync(ownerId);
            return Ok(plans);
        }

        [HttpGet("{id:guid}")]
        [Tags("Travel Plans")]
        /// <summary>
        /// Get a specific travel plan by ID
        /// </summary>
        public async Task<IActionResult> GetById(Guid id)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var plan = await _travelPlanService.GetByIdAsync(id, ownerId);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        // New: return only destinations for a plan
        [HttpGet("{planId:guid}/destinations")]
        [Tags("Destinations")]
        /// <summary>
        /// Get all destinations in a travel plan
        /// </summary>
        public async Task<IActionResult> GetDestinations(Guid planId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var destinations = await _travelPlanService.GetDestinationsAsync(planId, ownerId);
            if (destinations == null) return NotFound();

            return Ok(destinations);
        }

        [HttpGet("{planId:guid}/destinations/{destinationId:guid}")]
        [Tags("Destinations")]
        /// <summary>
        /// Get a specific destination
        /// </summary>
        public async Task<IActionResult> GetDestination(Guid planId, Guid destinationId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var destination = await _travelPlanService.GetDestinationAsync(planId, destinationId, ownerId);
            if (destination == null) return NotFound();

            return Ok(destination);
        }

        [HttpPut("{id:guid}")]
        [Tags("Travel Plans")]
        /// <summary>
        /// Update an existing travel plan
        /// </summary>
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateTravelPlanDTO dto)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            try
            {
                var updated = await _travelPlanService.UpdateAsync(id, dto, ownerId);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        [Tags("Travel Plans")]
        /// <summary>
        /// Delete a travel plan
        /// </summary>
        public async Task<IActionResult> Delete(Guid id)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var ok = await _travelPlanService.DeleteAsync(id, ownerId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:guid}/share")]
        [Tags("Sharing")]
        /// <summary>
        /// Generate a shareable link for a travel plan
        /// </summary>
        /// <param name="id">Travel plan ID</param>
        /// <param name="access">Access level: View or Edit</param>
        /// <param name="validDays">Number of days the link is valid</param>
        public async Task<IActionResult> Share(Guid id, [FromQuery] string access = "View", [FromQuery] int validDays = 7)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var token = await _travelPlanService.SharePlanAsync(id, ownerId, access, TimeSpan.FromDays(validDays));
            if (token == null) return NotFound();
            return Ok(new { token });
        }

        [HttpGet("{planId:guid}/share")]
        [Tags("Sharing")]
        /// <summary>
        /// Get all active share tokens for a travel plan
        /// </summary>
        public async Task<IActionResult> GetSharedTokens(Guid planId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var plan = await _travelPlanService.GetByIdAsync(planId, ownerId);
            if (plan == null) return NotFound();

            var tokens = await _travelPlanService.GetSharedTokensAsync(planId, ownerId);
            return Ok(tokens);
        }

        [HttpDelete("{planId:guid}/share/{tokenId:guid}")]
        [Tags("Sharing")]
        /// <summary>
        /// Delete a share token (revoke access)
        /// </summary>
        public async Task<IActionResult> DeleteSharedToken(Guid planId, Guid tokenId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var ok = await _travelPlanService.DeleteSharedTokenAsync(planId, tokenId, ownerId);
            if (!ok) return NotFound();
            return NoContent();
        }


        [AllowAnonymous]
        [HttpGet("shared/{token}")]
        [Tags("Sharing")]
        /// <summary>
        /// Access a shared travel plan by token
        /// </summary>
        public async Task<IActionResult> GetByShareToken(string token)
        {
            var (plan, accessLevel) = await _travelPlanService.GetByShareTokenAsync(token);
            if (plan == null) return NotFound();
            return Ok(new { plan, accessLevel });
        }

        [AllowAnonymous]
        [HttpGet("shared/{token}/qr")]
        [Tags("Sharing")]
        /// <summary>
        /// Generate QR code image for a shared plan token
        /// </summary>
        public IActionResult GetQRCode(string token)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var shareUrl = $"{baseUrl}/shared/{token}";

            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(shareUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);

            return File(qrCodeBytes, "image/png", $"qr-{token}.png");
        }

        // Sub-entity endpoints

        [HttpPost("{planId:guid}/destinations")]
        [Tags("Destinations")]
        /// <summary>
        /// Add a destination to a travel plan
        /// </summary>
        public async Task<IActionResult> AddDestination(Guid planId, [FromBody] DestinationDTO dto)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var created = await _travelPlanService.AddDestinationAsync(planId, dto, ownerId);
            if (created == null) return NotFound();
            return CreatedAtAction(nameof(GetById), new { id = planId }, created);
        }

        [HttpPut("{planId:guid}/destinations/{destinationId:guid}")]
        [Tags("Destinations")]
        /// <summary>
        /// Update a destination
        /// </summary>
        public async Task<IActionResult> UpdateDestination(Guid planId, Guid destinationId, [FromBody] DestinationDTO dto)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var updated = await _travelPlanService.UpdateDestinationAsync(planId, destinationId, dto, ownerId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{planId:guid}/destinations/{destinationId:guid}")]
        [Tags("Destinations")]
        /// <summary>
        /// Delete a destination
        /// </summary>
        public async Task<IActionResult> RemoveDestination(Guid planId, Guid destinationId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var ok = await _travelPlanService.RemoveDestinationAsync(planId, destinationId, ownerId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{planId:guid}/activities")]
        [Tags("Activities")]
        /// <summary>
        /// Add an activity to a travel plan
        /// </summary>
        public async Task<IActionResult> AddActivity(Guid planId, [FromBody] ActivityDTO dto)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var created = await _travelPlanService.AddActivityAsync(planId, dto, ownerId);
            if (created == null) return NotFound();
            return CreatedAtAction(nameof(GetActivity), new { planId = planId, activityId = created.Id }, created);
        }

        [HttpGet("{planId:guid}/activities")]
        [Tags("Activities")]
        /// <summary>
        /// Get all activities in a travel plan
        /// </summary>
        public async Task<IActionResult> GetActivities(Guid planId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var activities = await _travelPlanService.GetActivitiesAsync(planId, ownerId);
            if (activities == null) return NotFound();

            return Ok(activities);
        }

        [HttpGet("{planId:guid}/activities/{activityId:guid}")]
        [Tags("Activities")]
        /// <summary>
        /// Get a specific activity
        /// </summary>
        public async Task<IActionResult> GetActivity(Guid planId, Guid activityId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var activity = await _travelPlanService.GetActivityAsync(planId, activityId, ownerId);
            if (activity == null) return NotFound();

            return Ok(activity);
        }

        [HttpPut("{planId:guid}/activities/{activityId:guid}")]
        [Tags("Activities")]
        /// <summary>
        /// Update an activity
        /// </summary>
        public async Task<IActionResult> UpdateActivity(Guid planId, Guid activityId, [FromBody] ActivityDTO dto)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var updated = await _travelPlanService.UpdateActivityAsync(planId, activityId, dto, ownerId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{planId:guid}/activities/{activityId:guid}")]
        [Tags("Activities")]
        /// <summary>
        /// Delete an activity
        /// </summary>
        public async Task<IActionResult> RemoveActivity(Guid planId, Guid activityId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var ok = await _travelPlanService.RemoveActivityAsync(planId, activityId, ownerId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{planId:guid}/expenses")]
        [Tags("Expenses")]
        /// <summary>
        /// Add an expense to a travel plan
        /// </summary>
        public async Task<IActionResult> AddExpense(Guid planId, [FromBody] ExpenseDTO dto)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var created = await _travelPlanService.AddExpenseAsync(planId, dto, ownerId);
            if (created == null) return NotFound();
            return CreatedAtAction(nameof(GetExpense), new { planId = planId, expenseId = created.Id }, created);
        }

        [HttpGet("{planId:guid}/expenses")]
        [Tags("Expenses")]
        /// <summary>
        /// Get all expenses in a travel plan
        /// </summary>
        public async Task<IActionResult> GetExpenses(Guid planId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var expenses = await _travelPlanService.GetExpensesAsync(planId, ownerId);
            if (expenses == null) return NotFound();

            return Ok(expenses);
        }

        [HttpGet("{planId:guid}/expenses/{expenseId:guid}")]
        [Tags("Expenses")]
        /// <summary>
        /// Get a specific expense
        /// </summary>
        public async Task<IActionResult> GetExpense(Guid planId, Guid expenseId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var expense = await _travelPlanService.GetExpenseAsync(planId, expenseId, ownerId);
            if (expense == null) return NotFound();

            return Ok(expense);
        }

        [HttpPut("{planId:guid}/expenses/{expenseId:guid}")]
        [Tags("Expenses")]
        /// <summary>
        /// Update an expense
        /// </summary>
        public async Task<IActionResult> UpdateExpense(Guid planId, Guid expenseId, [FromBody] ExpenseDTO dto)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var updated = await _travelPlanService.UpdateExpenseAsync(planId, expenseId, dto, ownerId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{planId:guid}/expenses/{expenseId:guid}")]
        [Tags("Expenses")]
        /// <summary>
        /// Delete an expense
        /// </summary>
        public async Task<IActionResult> RemoveExpense(Guid planId, Guid expenseId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var ok = await _travelPlanService.RemoveExpenseAsync(planId, expenseId, ownerId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{planId:guid}/checklist")]
        [Tags("Checklist Items")]
        /// <summary>
        /// Add a checklist item to a travel plan
        /// </summary>
        public async Task<IActionResult> AddChecklistItem(Guid planId, [FromBody] ChecklistItemDTO dto)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var created = await _travelPlanService.AddChecklistItemAsync(planId, dto, ownerId);
            if (created == null) return NotFound();
            return CreatedAtAction(nameof(GetChecklistItem), new { planId = planId, checklistItemId = created.Id }, created);
        }

        [HttpGet("{planId:guid}/checklist")]
        [Tags("Checklist Items")]
        /// <summary>
        /// Get all checklist items in a travel plan
        /// </summary>
        public async Task<IActionResult> GetChecklistItems(Guid planId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var items = await _travelPlanService.GetChecklistItemsAsync(planId, ownerId);
            if (items == null) return NotFound();

            return Ok(items);
        }

        [HttpGet("{planId:guid}/checklist/{checklistItemId:guid}")]
        [Tags("Checklist Items")]
        /// <summary>
        /// Get a specific checklist item
        /// </summary>
        public async Task<IActionResult> GetChecklistItem(Guid planId, Guid checklistItemId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var item = await _travelPlanService.GetChecklistItemAsync(planId, checklistItemId, ownerId);
            if (item == null) return NotFound();

            return Ok(item);
        }

        [HttpPut("{planId:guid}/checklist/{checklistItemId:guid}")]
        [Tags("Checklist Items")]
        /// <summary>
        /// Update a checklist item
        /// </summary>
        public async Task<IActionResult> UpdateChecklistItem(Guid planId, Guid checklistItemId, [FromBody] ChecklistItemDTO dto)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var updated = await _travelPlanService.UpdateChecklistItemAsync(planId, checklistItemId, dto, ownerId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{planId:guid}/checklist/{checklistItemId:guid}")]
        [Tags("Checklist Items")]
        /// <summary>
        /// Delete a checklist item
        /// </summary>
        public async Task<IActionResult> RemoveChecklistItem(Guid planId, Guid checklistItemId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var ok = await _travelPlanService.RemoveChecklistItemAsync(planId, checklistItemId, ownerId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("{planId:guid}/activities/calendar")]
        [Tags("Activities")]
        /// <summary>
        /// Get an activity calendar view for a travel plan
        /// </summary>
        public async Task<IActionResult> GetActivitiesCalendar(Guid planId)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == Guid.Empty) return Unauthorized();

            var calendar = await _travelPlanService.GetActivitiesCalendarAsync(planId, ownerId);
            if (calendar == null) return NotFound();
            return Ok(calendar);
        }
    }
}