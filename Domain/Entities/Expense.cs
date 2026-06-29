using System;

namespace WebProjekat.Domain.Entities
{
    public class Expense
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid TravelPlanId { get; private set; }
        public string Name { get; private set; }
        public string Category { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public string Description { get; private set; }

        private Expense() { }

        public Expense(Guid travelPlanId, string name, string category, decimal amount, DateTime date, string description)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative.");
            TravelPlanId = travelPlanId;
            Name = name;
            Category = category;
            Amount = amount;
            Date = date;
            Description = description;
        }

        public void Update(string name, string category, decimal amount, DateTime date, string description)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
            if (amount < 0) throw new ArgumentException("Amount cannot be negative.");
            Name = name;
            Category = category;
            Amount = amount;
            Date = date;
            Description = description;
        }
    }
}