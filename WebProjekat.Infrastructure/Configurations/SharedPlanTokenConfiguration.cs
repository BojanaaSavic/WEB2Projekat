using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebProjekat.Domain.Entities;

namespace WebProjekat.Infrastructure.Configurations
{
    public class SharedPlanTokenConfiguration : IEntityTypeConfiguration<SharedPlanToken>
    {
        public void Configure(EntityTypeBuilder<SharedPlanToken> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Token).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();
        }
    }
}