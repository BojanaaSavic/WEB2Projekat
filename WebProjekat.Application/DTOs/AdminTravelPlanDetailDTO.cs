using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProjekat.Application.DTOs
{
    public class AdminTravelPlanDetailDTO : AdminTravelPlanDTO
    {
        public decimal TotalExpenses { get; set; }
        public int DestinationCount { get; set; }
        public int ActivityCount { get; set; }
        public int ExpenseCount { get; set; }
        public int ChecklistItemCount { get; set; }
    }
}
