using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProjekat.Application.DTOs
{
    public class AdminStatisticsDTO
    {
        public int? TotalUsers { get; set; }
        public int? TotalPlans { get; set; }
        public int? TotalDestinations { get; set; }
        public int? TotalActivities { get; set; }
        public decimal? TotalExpenses { get; set; }
    }
}
