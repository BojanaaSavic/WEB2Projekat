using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;

namespace WebProjekat.Application.DTOs
{
    public class TravelPlanDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PlannedBudget { get; set; }
        public string Notes { get; set; }
        public Guid OwnerId { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal RemainingBudget { get; set; }

        public ICollection<DestinationDTO> Destinations { get; set; } = new List<DestinationDTO>();
        public ICollection<ActivityDTO> Activities { get; set; } = new List<ActivityDTO>();
        public ICollection<ExpenseDTO> Expenses { get; set; } = new List<ExpenseDTO>();
        public ICollection<ChecklistItemDTO> ChecklistItems { get; set; } = new List<ChecklistItemDTO>();
    }
}