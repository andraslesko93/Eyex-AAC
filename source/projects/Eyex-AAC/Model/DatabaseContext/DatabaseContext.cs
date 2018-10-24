﻿using Microsoft.EntityFrameworkCore;
namespace EyexAAC.Model
{
    class DatabaseContext : DbContext
    {
        private static bool _created = false;

        public DatabaseContext()
        {
            if (!_created)
            {
                _created = true;
               // Database.EnsureDeleted();
                Database.EnsureCreated();
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
            => optionbuilder.UseLazyLoadingProxies().UseSqlite(@"Data Source=Sample.db");

        public DbSet<Messenger> Messengers { get; set; }
        public DbSet<ActivityLogEntry> ActivityLog { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
