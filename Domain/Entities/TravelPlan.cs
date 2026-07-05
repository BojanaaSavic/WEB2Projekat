using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace WebProjekat.Domain.Entities
{
    public class TravelPlan
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public decimal PlannedBudget { get; private set; }
        public string Notes { get; private set; }
        public Guid OwnerId { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Destination> Destinations { get; private set; } = new List<Destination>();
        public ICollection<Activity> Activities { get; private set; } = new List<Activity>();
        public ICollection<ChecklistItem> ChecklistItems { get; private set; } = new List<ChecklistItem>();
        public ICollection<SharedPlanToken> SharedTokens { get; private set; } = new List<SharedPlanToken>();

        // Required by EF
        private TravelPlan() { }

        public TravelPlan(string name, string description, DateTime startDate, DateTime endDate, decimal plannedBudget, string notes, Guid ownerId)
        {
            Update(name, description, startDate, endDate, plannedBudget, notes, ownerId);
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string description, DateTime startDate, DateTime endDate, decimal plannedBudget, string notes, Guid ownerId)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
            if (startDate.Date < DateTime.UtcNow.Date) throw new ArgumentException("Start date cannot be in the past.");
            if (endDate.Date < startDate.Date) throw new ArgumentException("End date cannot be before start date.");
            if (plannedBudget < 0) throw new ArgumentException("Planned budget cannot be negative.");

            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            PlannedBudget = plannedBudget;
            Notes = notes;
            OwnerId = ownerId;
        }

        public void AddDestination(Destination d) => Destinations.Add(d);
        public void AddActivity(Activity a) => Activities.Add(a);
        //public void AddExpense(Expense e) => Expenses.Add(e);
        public void AddChecklistItem(ChecklistItem c) => ChecklistItems.Add(c);
        public void AddSharedToken(SharedPlanToken t) => SharedTokens.Add(t);
    }
}