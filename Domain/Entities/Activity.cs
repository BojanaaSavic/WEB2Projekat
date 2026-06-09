using System;

namespace WebProjekat.Domain.Entities
{
    public enum ActivityStatus
    {
        Planned,
        Reserved,
        Completed,
        Cancelled
    }

    public class Activity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid TravelPlanId { get; private set; }
        public string Name { get; private set; }
        public DateTime ScheduledAt { get; private set; }
        public string Location { get; private set; }
        public string Description { get; private set; }
        public decimal EstimatedCost { get; private set; }
        public ActivityStatus Status { get; private set; }

        public TravelPlan TravelPlan { get; private set; }

        private Activity() { }

        public Activity(Guid travelPlanId, string name, DateTime scheduledAt, string location, string description, decimal estimatedCost, ActivityStatus status)
        {
            if (estimatedCost < 0) throw new ArgumentException("Estimated cost cannot be negative.");
            TravelPlanId = travelPlanId;
            Name = name;
            ScheduledAt = scheduledAt;
            Location = location;
            Description = description;
            EstimatedCost = estimatedCost;
            Status = status;
        }

        public void Update(string name, DateTime scheduledAt, string location, string description, decimal estimatedCost, ActivityStatus status)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
            if (estimatedCost < 0) throw new ArgumentException("Estimated cost cannot be negative.");
            Name = name;
            ScheduledAt = scheduledAt;
            Location = location;
            Description = description;
            EstimatedCost = estimatedCost;
            Status = status;
        }
    }
}