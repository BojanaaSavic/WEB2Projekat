using System;
using System.ComponentModel.DataAnnotations;

namespace WebProjekat.Application.DTOs
{
    /// <summary>
    /// Expense information for a travel plan
    /// </summary>
    public class ExpenseDTO
    {
        /// <summary>
        /// Unique identifier for the expense
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name or description of the expense
        /// </summary>
        /// <example>Hotel accommodation</example>
        [Required(ErrorMessage = "Expense name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Category of the expense
        /// </summary>
        /// <example>Accommodation</example>
        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; }

        /// <summary>
        /// Amount spent
        /// </summary>
        /// <example>150.00</example>
        [Range(0, double.MaxValue, ErrorMessage = "Amount cannot be negative.")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Date when the expense was incurred
        /// </summary>
        /// <example>2024-07-01</example>
        [Required(ErrorMessage = "Expense date is required.")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Additional details about the expense
        /// </summary>
        /// <example>3 nights at Hotel de Paris</example>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }
    }
}