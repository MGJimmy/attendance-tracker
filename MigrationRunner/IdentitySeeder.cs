using Identity.Service;
using Microsoft.AspNetCore.Identity;

namespace Printpress.MigrationRunner;

internal sealed class IdentitySeeder
{
    private const string AdminUserName = "admin";
    private const string AdminEmail = "admin@attendancetracker.local";
    private const string AdminPassword = "1q2w3E*";

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentitySeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAdminAsync()
    {
        await EnsureRoleAsync(RoleName.Admin);
        await EnsureRoleAsync(RoleName.User);

        var admin = await _userManager.FindByNameAsync(AdminUserName);
        if (admin is null)
        {
            admin = new User
            {
                UserName = AdminUserName,
                Email = AdminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(admin, AdminPassword);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to create admin user: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }

        if (!await _userManager.IsInRoleAsync(admin, RoleName.Admin))
        {
            var roleResult = await _userManager.AddToRoleAsync(admin, RoleName.Admin);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to assign Admin role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }
    }

    private async Task EnsureRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
            return;

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create role '{roleName}': " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
