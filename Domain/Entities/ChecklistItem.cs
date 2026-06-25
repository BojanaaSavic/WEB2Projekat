using System;

namespace WebProjekat.Domain.Entities
{
    public class ChecklistItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid TravelPlanId { get; private set; }
        public string Title { get; private set; }
        public bool IsCompleted { get; private set; }
        public string Description { get; private set; }

        public TravelPlan TravelPlan { get; private set; }

        private ChecklistItem() { }

        public ChecklistItem(Guid travelPlanId, string title, bool isCompleted, string description)
        {
            TravelPlanId = travelPlanId;
            Title = title;
            IsCompleted = isCompleted;
            Description = description;
        }

        public void SetCompleted(bool done) => IsCompleted = done;

        public void Update(string title, bool isCompleted, string description)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Name is required.", nameof(title));
            Title = title;
            IsCompleted = isCompleted;
            Description = description;
        }
    }
}