using System;
using System.Collections.Generic;

namespace WebProjekat.Application.DTOs
{
    public class ActivityCalendarDayDTO
    {
        public DateTime Date { get; set; }
        public ICollection<ActivityDTO> Activities { get; set; } = new List<ActivityDTO>();
    }
}