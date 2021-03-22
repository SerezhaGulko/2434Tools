using _2434Tools.Data;
using _2434ToolsUser.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2434Tools
{
    public static class DbMigration
    {
        public static IWebHost MigrateDatabase(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();
                DbMigration.ConfigureIdentity(scope).GetAwaiter().GetResult();
            }

            return webHost;
        }

        private static async Task ConfigureIdentity(IServiceScope scope)
        {
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            var adminsRole = await roleManager.FindByNameAsync(ApplicationRoles.Administrators);
            if (adminsRole == null)
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Administrators));
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException("Could not create admin role");
                }

                adminsRole = await roleManager.FindByNameAsync(ApplicationRoles.Administrators);
            }
            const String adminName      = "Administrator2434";
            const String adminEmail     = "Administrator2434@localhost.local";
            const String adminPassword  = "Administrator2434!!";
            var adminUser = await userManager.FindByNameAsync(adminName);
            if (adminUser == null)
            {
                var userResult = await userManager.CreateAsync(new ApplicationUser
                {
                    UserName    = adminName,
                    Email       = adminEmail
                }, adminPassword);
                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException("Could not create admin user");
                }

                adminUser = await userManager.FindByNameAsync(adminName);
            }

            if (!await userManager.IsInRoleAsync(adminUser, adminsRole.Name))
            {
                await userManager.AddToRoleAsync(adminUser, adminsRole.Name);
            }
        }
    }
}
