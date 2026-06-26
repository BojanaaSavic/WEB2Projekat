using Microsoft.EntityFrameworkCore;
using WebProjekat.Domain.Entities;
using WebProjekat.Infrastructure.Configurations;

namespace WebProjekat.Infrastructure.Persistence
{
    public class TravelPlanDbContext : DbContext
    {
        public TravelPlanDbContext(DbContextOptions<TravelPlanDbContext> options) : base(options) { }

        public DbSet<TravelPlan> TravelPlans { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ChecklistItem> ChecklistItems { get; set; }
        public DbSet<SharedPlanToken> SharedPlanTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TravelPlanConfiguration());
            modelBuilder.ApplyConfiguration(new DestinationConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityConfiguration());
            modelBuilder.ApplyConfiguration(new ChecklistItemConfiguration());
            modelBuilder.ApplyConfiguration(new SharedPlanTokenConfiguration());
        }
    }
}