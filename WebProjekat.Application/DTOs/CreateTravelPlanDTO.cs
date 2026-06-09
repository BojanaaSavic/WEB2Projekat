using System;

namespace WebProjekat.Application.DTOs
{
    /// <summary>
    /// Data transfer object for creating or updating a travel plan
    /// </summary>
    public class CreateTravelPlanDTO
    {
        /// <summary>
        /// Name of the travel plan
        /// </summary>
        /// <example>Summer Vacation 2024</example>
        public string Name { get; set; }

        /// <summary>
        /// Brief description of the trip
        /// </summary>
        /// <example>Two-week trip to Europe visiting Paris, Rome, and Barcelona</example>
        public string Description { get; set; }

        /// <summary>
        /// Travel start date
        /// </summary>
        /// <example>2024-07-01</example>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Travel end date
        /// </summary>
        /// <example>2024-07-14</example>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Planned budget for the trip
        /// </summary>
        /// <example>5000.00</example>
        public decimal PlannedBudget { get; set; }

        /// <summary>
        /// Additional notes about the trip
        /// </summary>
        /// <example>Remember to book hotels in advance</example>
        public string Notes { get; set; }
    }
}