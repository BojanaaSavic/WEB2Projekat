using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WebProjekat.Infrastructure.Persistence
{
    public class TravelPlanDbContextFactory : IDesignTimeDbContextFactory<TravelPlanDbContext>
    {
        public TravelPlanDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TravelPlanService"))
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TravelPlanDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("TravelPlanConnection"));

            return new TravelPlanDbContext(optionsBuilder.Options);
        }
    }
}