using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebProjekat.Application.DTOs;

namespace WebProjekat.Application.Interfaces
{
    public interface ITravelPlanService
    {
        Task<TravelPlanDTO> CreateAsync(CreateTravelPlanDTO dto, Guid ownerId);
        Task<IEnumerable<TravelPlanDTO>> GetAllForUserAsync(Guid ownerId);
        Task<TravelPlanDTO> GetByIdAsync(Guid id, Guid ownerId);
        Task<TravelPlanDTO> UpdateAsync(Guid id, CreateTravelPlanDTO dto, Guid ownerId);
        Task<bool> DeleteAsync(Guid id, Guid ownerId);

        Task<string> SharePlanAsync(Guid id, Guid ownerId, string accessLevel, TimeSpan validFor);

        // Sub-entity management
        Task<DestinationDTO> AddDestinationAsync(Guid planId, DestinationDTO dto, Guid ownerId);
        Task<DestinationDTO> GetDestinationAsync(Guid planId, Guid destinationId, Guid ownerId);
        Task<IEnumerable<DestinationDTO>> GetDestinationsAsync(Guid planId, Guid ownerId);
        Task<DestinationDTO> UpdateDestinationAsync(Guid planId, Guid destinationId, DestinationDTO dto, Guid ownerId);
        Task<bool> RemoveDestinationAsync(Guid planId, Guid destinationId, Guid ownerId);

        Task<ActivityDTO> AddActivityAsync(Guid planId, ActivityDTO dto, Guid ownerId);
        Task<ActivityDTO> GetActivityAsync(Guid planId, Guid activityId, Guid ownerId);
        Task<IEnumerable<ActivityDTO>> GetActivitiesAsync(Guid planId, Guid ownerId);
        Task<ActivityDTO> UpdateActivityAsync(Guid planId, Guid activityId, ActivityDTO dto, Guid ownerId);
        Task<bool> RemoveActivityAsync(Guid planId, Guid activityId, Guid ownerId);

        Task<ExpenseDTO> AddExpenseAsync(Guid planId, ExpenseDTO dto, Guid ownerId);
        Task<ExpenseDTO> GetExpenseAsync(Guid planId, Guid expenseId, Guid ownerId);
        Task<IEnumerable<ExpenseDTO>> GetExpensesAsync(Guid planId, Guid ownerId);
        Task<ExpenseDTO> UpdateExpenseAsync(Guid planId, Guid expenseId, ExpenseDTO dto, Guid ownerId);
        Task<bool> RemoveExpenseAsync(Guid planId, Guid expenseId, Guid ownerId);

        Task<ChecklistItemDTO> AddChecklistItemAsync(Guid planId, ChecklistItemDTO dto, Guid ownerId);
        Task<ChecklistItemDTO> GetChecklistItemAsync(Guid planId, Guid checklistItemId, Guid ownerId);
        Task<IEnumerable<ChecklistItemDTO>> GetChecklistItemsAsync(Guid planId, Guid ownerId);
        Task<ChecklistItemDTO> UpdateChecklistItemAsync(Guid planId, Guid checklistItemId, ChecklistItemDTO dto, Guid ownerId);
        Task<bool> RemoveChecklistItemAsync(Guid planId, Guid checklistItemId, Guid ownerId);

        // Access by share token
        Task<(TravelPlanDTO Plan, string AccessLevel)> GetByShareTokenAsync(string token);
        Task<IEnumerable<SharedTokenDTO>> GetSharedTokensAsync(Guid planId, Guid ownerId);
        Task<bool> DeleteSharedTokenAsync(Guid planId, Guid tokenId, Guid ownerId);

        Task<IEnumerable<ActivityCalendarDayDTO>> GetActivitiesCalendarAsync(Guid planId, Guid ownerId);
    }
}