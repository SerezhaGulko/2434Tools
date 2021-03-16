using _2434Tools.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace _2434Tools
{
    public static class DbMigration
    {
        public static IWebHost MigrateDatabase(this IWebHost webHost)
        {
            using(var scope = webHost.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<AppDbContext>();
                context.Database.Migrate();
            }

            return webHost;
        }
    }
}
