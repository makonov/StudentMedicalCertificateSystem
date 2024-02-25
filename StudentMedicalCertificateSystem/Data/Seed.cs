using Microsoft.AspNetCore.Identity;
using StudentMedicalCertificateSystem.Models;
using System.Diagnostics;
using System.Net;

namespace StudentMedicalCertificateSystem.Data
{
    public class Seed
    {
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

                // Ensure the database is created
                dbContext.Database.EnsureCreated();

                // Roles
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.Guest))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Guest));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                // Users
                string adminUserEmail = "admin@gmail.com";
                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);

                if (adminUser == null)
                {
                    var newAdminUser = new User()
                    {
                        UserName = "admin",
                        Email = adminUserEmail,
                        EmailConfirmed = true,
                        FirstName = "TEST",
                        LastName = "TEST",
                        Patronymic = "TEST"
                    };

                    await userManager.CreateAsync(newAdminUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                string appGuestEmail = "user@etickets.com";
                var appGuest = await userManager.FindByEmailAsync(appGuestEmail);

                if (appGuest == null)
                {
                    var newAppUser = new User()
                    {
                        UserName = "professor",
                        Email = appGuestEmail,
                        EmailConfirmed = true,
                        FirstName = "TEST",
                        LastName = "TEST",
                        Patronymic = "TEST"
                    };

                    await userManager.CreateAsync(newAppUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.Guest);
                }

                string appUserEmail = "user@example.com";
                var appuser = await userManager.FindByEmailAsync(appUserEmail);

                if (appuser == null)
                {
                    var newAppUser = new User()
                    {
                        UserName = "user",
                        Email = appUserEmail,
                        EmailConfirmed = true,
                        FirstName = "TEST",
                        LastName = "TEST",
                        Patronymic = "TEST"
                    };

                    await userManager.CreateAsync(newAppUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }
            }
        }
    }
}
