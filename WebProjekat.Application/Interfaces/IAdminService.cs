using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebProjekat.Application.DTOs;

namespace WebProjekat.Application.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<AdminUserDTO>> GetAllUsersAsync();
        Task<AdminUserDTO> GetUserAsync(Guid userId);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<IEnumerable<AdminTravelPlanDTO>> GetAllTravelPlansAsync();
        Task<AdminTravelPlanDetailDTO> GetTravelPlanAsync(Guid planId);
        Task<bool> DeleteTravelPlanAsync(Guid planId);
        Task<AdminStatisticsDTO> GetStatisticsAsync();
    }
}
