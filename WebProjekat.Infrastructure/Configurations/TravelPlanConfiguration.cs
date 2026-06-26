using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebProjekat.Domain.Entities;

namespace WebProjekat.Infrastructure.Configurations
{
    public class TravelPlanConfiguration : IEntityTypeConfiguration<TravelPlan>
    {
        public void Configure(EntityTypeBuilder<TravelPlan> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(2000);
            builder.Property(x => x.PlannedBudget).HasColumnType("decimal(18,2)");

            builder.HasMany(x => x.Destinations)
                   .WithOne(d => d.TravelPlan)
                   .HasForeignKey(d => d.TravelPlanId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Activities)
                   .WithOne(a => a.TravelPlan)
                   .HasForeignKey(a => a.TravelPlanId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ChecklistItems)
                   .WithOne(c => c.TravelPlan)
                   .HasForeignKey(c => c.TravelPlanId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.SharedTokens)
                   .WithOne(s => s.TravelPlan)
                   .HasForeignKey(s => s.TravelPlanId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}