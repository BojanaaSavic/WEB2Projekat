using System;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Travel plan name is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Brief description of the trip
        /// </summary>
        /// <example>Two-week trip to Europe visiting Paris, Rome, and Barcelona</example>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        /// <summary>
        /// Travel start date
        /// </summary>
        /// <example>2024-07-01</example>
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Travel end date
        /// </summary>
        /// <example>2024-07-14</example>
        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Planned budget for the trip
        /// </summary>
        /// <example>5000.00</example>
        [Range(0, double.MaxValue, ErrorMessage = "Budget cannot be negative.")]
        public decimal PlannedBudget { get; set; }

        /// <summary>
        /// Additional notes about the trip
        /// </summary>
        /// <example>Remember to book hotels in advance</example>
        [StringLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters.")]
        public string Notes { get; set; }
    }
}