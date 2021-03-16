using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace _2434Tools.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions options) :
            base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
