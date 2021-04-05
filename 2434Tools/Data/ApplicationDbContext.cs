using _2434Tools.Models;
using _2434ToolsUser.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _2434Tools.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Liver> Livers  { get; set; }
        public DbSet<Video> Videos  { get; set; }
        public DbSet<Group> Groups  { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Liver>().HasOne(_liver => _liver.Group).WithMany(_group => _group.Livers).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
