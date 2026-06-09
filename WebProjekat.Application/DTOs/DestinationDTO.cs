using System;

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
        public string Name { get; set; }

        /// <summary>
        /// Location or city of the destination
        /// </summary>
        /// <example>France</example>
        public string Location { get; set; }

        /// <summary>
        /// Expected arrival date at this destination
        /// </summary>
        /// <example>2024-07-01</example>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Expected departure date from this destination
        /// </summary>
        /// <example>2024-07-04</example>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Description of the destination
        /// </summary>
        /// <example>The City of Light - famous for the Eiffel Tower and museums</example>
        public string Description { get; set; }
    }
}