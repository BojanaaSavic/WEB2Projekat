using System;
using System.ComponentModel.DataAnnotations;

namespace WebProjekat.Application.DTOs
{
    /// <summary>
    /// Checklist item for travel planning
    /// </summary>
    public class ChecklistItemDTO
    {
        /// <summary>
        /// Unique identifier for the checklist item
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Task or item to be completed
        /// </summary>
        /// <example>Book flights</example>
        [Required(ErrorMessage = "Checklist item title is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// Whether the task has been completed
        /// </summary>
        /// <example>false</example>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Details about the checklist item
        /// </summary>
        /// <example>Book flights for the upcoming aniversary</example>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }
    }
}