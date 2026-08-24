using AttendanceTracker.Infrastructure;
using Identity.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Printpress.MigrationRunner;

try
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json")
        .Build();

    var serviceProvider = new ServiceCollection()
        .AddLogging()
        .AddDbContext<AppDBContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")))
        .AddUserServices(configuration)
        .AddScoped<IdentitySeeder>()
        .BuildServiceProvider();

    using var scope = serviceProvider.CreateScope();

    var appDbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();

    await appDbContext.Database.MigrateAsync();
    await identityDbContext.Database.MigrateAsync();
    await seeder.SeedAdminAsync();

    Console.WriteLine("Database migrations and admin seeding completed successfully.");
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred while applying migrations: " + ex.Message);
    Console.WriteLine(ex);
}
