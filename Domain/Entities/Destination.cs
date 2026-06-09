using System;

namespace WebProjekat.Domain.Entities
{
    public class Destination
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid TravelPlanId { get; private set; }
        public string Name { get; private set; }
        public string Location { get; private set; }
        public DateTime ArrivalDate { get; private set; }
        public DateTime DepartureDate { get; private set; }
        public string Description { get; private set; }

        // Navigation
        public TravelPlan TravelPlan { get; private set; }

        private Destination() { }

        public Destination(Guid travelPlanId, string name, string location, DateTime arrival, DateTime departure, string description)
        {
            if (departure < arrival) throw new ArgumentException("Departure cannot be before arrival.");
            TravelPlanId = travelPlanId;
            Name = name;
            Location = location;
            ArrivalDate = arrival;
            DepartureDate = departure;
            Description = description;
        }

        public void Update(string name, string location, DateTime arrival, DateTime departure, string description)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
            if (departure < arrival) throw new ArgumentException("Departure cannot be before arrival.");
            Name = name;
            Location = location;
            ArrivalDate = arrival;
            DepartureDate = departure;
            Description = description;
        }
    }
}