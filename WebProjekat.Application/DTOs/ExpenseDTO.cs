using System;

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
        public string Name { get; set; }

        /// <summary>
        /// Category of the expense
        /// </summary>
        /// <example>Accommodation</example>
        public string Category { get; set; }

        /// <summary>
        /// Amount spent
        /// </summary>
        /// <example>150.00</example>
        public decimal Amount { get; set; }

        /// <summary>
        /// Date when the expense was incurred
        /// </summary>
        /// <example>2024-07-01</example>
        public DateTime Date { get; set; }

        /// <summary>
        /// Additional details about the expense
        /// </summary>
        /// <example>3 nights at Hotel de Paris</example>
        public string Description { get; set; }
    }
}