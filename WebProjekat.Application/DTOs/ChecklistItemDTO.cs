using System;

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
        public string Name { get; set; }

        /// <summary>
        /// Whether the task has been completed
        /// </summary>
        /// <example>false</example>
        public bool IsCompleted { get; set; }
    }
}