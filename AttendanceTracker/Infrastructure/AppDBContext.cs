using AttendanceTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace AttendanceTracker.Infrastructure
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.UserId).IsRequired();
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.Property(e => e.SalaryPerHour).HasColumnType("numeric");
            });

            modelBuilder.Entity<Courier>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MobileNumber).IsRequired().HasMaxLength(30);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.MobileNumber).IsUnique();
            });

            modelBuilder.Entity<Destination>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Details).HasMaxLength(500);
                entity.Property(e => e.Cost).HasColumnType("numeric");
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.Property(e => e.Cost).HasColumnType("numeric");
                entity.Property(e => e.TripDate).HasColumnType("date");

                entity.HasOne(e => e.Courier)
                    .WithMany(e => e.Trips)
                    .HasForeignKey(e => e.CourierId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Destination)
                    .WithMany(e => e.Trips)
                    .HasForeignKey(e => e.DestinationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.TripDate);
                entity.HasIndex(e => e.CourierId);
            });
        }
    }
}
