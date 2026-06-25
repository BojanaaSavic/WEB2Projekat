using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProjekat.Application.DTOs
{
    public class AdminTravelPlanDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PlannedBudget { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
