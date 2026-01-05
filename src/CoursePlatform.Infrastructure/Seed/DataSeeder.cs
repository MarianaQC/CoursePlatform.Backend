using CoursePlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CoursePlatform.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "User" };

        foreach (var roleName in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var adminEmail = "marianaqc64@gmail.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Mariana",
                LastName = "QC",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var result = await userManager.CreateAsync(adminUser, "osfigprczoilwdvw");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}