using Microsoft.EntityFrameworkCore;
using WebProjekat.Domain.Entities;
using WebProjekat.Infrastructure.Configurations;

namespace WebProjekat.Infrastructure.Persistence
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options) { }

        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
        }
    }
}