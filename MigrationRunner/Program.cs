using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Printpress.Infrastructure;
using Printpress.MigrationRunner;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            // Build configuration (e.g., from appsettings.json)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Set up dependency injection
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")))
                .AddScoped<SeedingDbContext>()
                .BuildServiceProvider();

            // Resolve the DbContext and run database migrations
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var seedingDbContext = scope.ServiceProvider.GetRequiredService<SeedingDbContext>();


                // Apply migrations
                dbContext.Database.Migrate();

                //use this in development only
                //seedingDbContext.SeedingMockData();

                //use this to add lockup data
                seedingDbContext.SeedingData();

                //call save changes only here to save the data dont call it in the seeding methods
                dbContext.CurrentUserId = "Seeding";
                dbContext.SaveChanges();

                Console.WriteLine("Database migrations applied successfully.");

                Console.ReadLine();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while applying migrations: " + ex.Message);
        }



    }
}

