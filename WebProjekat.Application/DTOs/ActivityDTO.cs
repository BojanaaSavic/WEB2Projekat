using System;
using System.ComponentModel.DataAnnotations;
using WebProjekat.Domain.Entities;

namespace WebProjekat.Application.DTOs
{
    /// <summary>
    /// Activity information for a travel plan
    /// </summary>
    public class ActivityDTO
    {
        /// <summary>
        /// Unique identifier for the activity
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the activity
        /// </summary>
        /// <example>Visit Eiffel Tower</example>
        [Required(ErrorMessage = "Activity name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Date and time when the activity is scheduled
        /// </summary>
        /// <example>2024-07-02T09:00:00</example>
        [Required(ErrorMessage = "Scheduled date and time is required.")]
        public DateTime ScheduledAt { get; set; }

        /// <summary>
        /// Location where the activity takes place
        /// </summary>
        /// <example>Eiffel Tower, Champ de Mars, 5 Avenue Anatole France, 75007 Paris</example>
        [StringLength(300, ErrorMessage = "Location cannot exceed 300 characters.")]
        public string Location { get; set; }

        /// <summary>
        /// Description of the activity
        /// </summary>
        /// <example>Climb to the top of the Eiffel Tower for panoramic views of Paris</example>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        /// <summary>
        /// Estimated cost of the activity
        /// </summary>
        /// <example>25.50</example>
        [Range(0, double.MaxValue, ErrorMessage = "Estimated cost cannot be negative.")]
        public decimal EstimatedCost { get; set; }

        /// <summary>
        /// Current status of the activity
        /// </summary>
        /// <example>Planned</example>
        [Required(ErrorMessage = "Activity status is required.")]
        public ActivityStatus Status { get; set; }
    }
}