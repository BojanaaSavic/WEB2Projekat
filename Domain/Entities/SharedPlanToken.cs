using System;

namespace WebProjekat.Domain.Entities
{
    public enum ShareAccessLevel
    {
        View,
        Edit
    }

    public class SharedPlanToken
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid TravelPlanId { get; private set; }
        public string Token { get; private set; }
        public ShareAccessLevel AccessLevel { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public TravelPlan TravelPlan { get; private set; }

        private SharedPlanToken() { }

        public SharedPlanToken(Guid travelPlanId, ShareAccessLevel accessLevel, TimeSpan validFor)
        {
            TravelPlanId = travelPlanId;
            Token = Guid.NewGuid().ToString("N");
            AccessLevel = accessLevel;
            ExpiresAt = DateTime.UtcNow.Add(validFor);
        }

        public bool IsValid() => DateTime.UtcNow <= ExpiresAt;
    }
}