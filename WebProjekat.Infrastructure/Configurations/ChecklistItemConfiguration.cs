using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebProjekat.Domain.Entities;

namespace WebProjekat.Infrastructure.Configurations
{
    public class ChecklistItemConfiguration : IEntityTypeConfiguration<ChecklistItem>
    {
        public void Configure(EntityTypeBuilder<ChecklistItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(300);
        }
    }
}