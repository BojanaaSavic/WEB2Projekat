using System;
using System.ComponentModel.DataAnnotations;

namespace WebProjekat.Application.DTOs
{
    /// <summary>
    /// Destination information for a travel plan
    /// </summary>
    public class DestinationDTO
    {
        /// <summary>
        /// Unique identifier for the destination
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the destination
        /// </summary>
        /// <example>Paris</example>
        [Required(ErrorMessage = "Destination name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Location or city of the destination
        /// </summary>
        /// <example>France</example>
        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; }

        /// <summary>
        /// Expected arrival date at this destination
        /// </summary>
        /// <example>2024-07-01</example>
        [Required(ErrorMessage = "Arrival date is required.")]
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Expected departure date from this destination
        /// </summary>
        /// <example>2024-07-04</example>
        [Required(ErrorMessage = "Departure date is required.")]
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Description of the destination
        /// </summary>
        /// <example>The City of Light - famous for the Eiffel Tower and museums</example>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }
    }
}