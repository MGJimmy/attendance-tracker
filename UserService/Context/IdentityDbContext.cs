using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Service
{
    public class IdentityDbContext : IdentityDbContext<User>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        public new DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Identity");

            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
               new IdentityRole
               {
                   Id = "1", // Ensure unique string
                   Name = RoleName.Admin,
                   NormalizedName = RoleName.Admin.ToUpper()
               },
               new IdentityRole
               {
                   Id = "2",
                   Name = RoleName.User,
                   NormalizedName = RoleName.User.ToUpper()
               }
            );

            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasOne(e => e.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        }

    }
}
