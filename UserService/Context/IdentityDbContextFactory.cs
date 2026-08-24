using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Identity.Service
{
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var current = Directory.GetCurrentDirectory();
            var apiPath = Path.Combine(current, "appsettings.json");
            var siblingPath = Path.GetFullPath(Path.Combine(current, "..", "AttendanceTracker"));

            var basePath = File.Exists(apiPath) ? current : siblingPath;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            var connectionString = configuration.GetConnectionString("UserConnectionString");
            optionsBuilder.UseNpgsql(connectionString);

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}
