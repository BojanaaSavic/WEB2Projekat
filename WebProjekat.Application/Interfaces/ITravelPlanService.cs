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
        Task<TravelPlanDTO> UpdateViaShareTokenAsync(string token, CreateTravelPlanDTO dto, Guid editorUserId);
        Task<DestinationDTO> AddDestinationViaShareTokenAsync(string token, DestinationDTO dto);
        Task<DestinationDTO> UpdateDestinationViaShareTokenAsync(string token, Guid destinationId, DestinationDTO dto);

        Task<ActivityDTO> AddActivityViaShareTokenAsync(string token, ActivityDTO dto);
        Task<ActivityDTO> UpdateActivityViaShareTokenAsync(string token, Guid activityId, ActivityDTO dto);

        Task<ChecklistItemDTO> AddChecklistItemViaShareTokenAsync(string token, ChecklistItemDTO dto);
        Task<ChecklistItemDTO> UpdateChecklistItemViaShareTokenAsync(string token, Guid checklistItemId, ChecklistItemDTO dto);
        Task<bool> RemoveDestinationViaShareTokenAsync(string token, Guid destinationId);
        Task<bool> RemoveActivityViaShareTokenAsync(string token, Guid activityId);
        Task<bool> RemoveChecklistItemViaShareTokenAsync(string token, Guid checklistItemId);

        // Admin
        Task<IEnumerable<AdminTravelPlanDTO>> GetAllPlansAdminAsync();
        Task<bool> DeletePlanAdminAsync(Guid planId);
        Task<AdminStatisticsDTO> GetAdminStatisticsAsync();
        Task DeletePlansByOwnerAsync(Guid ownerId);
    }
}