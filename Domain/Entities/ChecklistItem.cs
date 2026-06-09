using System;

namespace WebProjekat.Domain.Entities
{
    public class ChecklistItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid TravelPlanId { get; private set; }
        public string Name { get; private set; }
        public bool IsCompleted { get; private set; }

        public TravelPlan TravelPlan { get; private set; }

        private ChecklistItem() { }

        public ChecklistItem(Guid travelPlanId, string name, bool isCompleted)
        {
            TravelPlanId = travelPlanId;
            Name = name;
            IsCompleted = isCompleted;
        }

        public void SetCompleted(bool done) => IsCompleted = done;

        public void Update(string name, bool isCompleted)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
            Name = name;
            IsCompleted = isCompleted;
        }
    }
}