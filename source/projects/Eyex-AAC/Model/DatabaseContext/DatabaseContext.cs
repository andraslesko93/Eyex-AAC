using Microsoft.EntityFrameworkCore;
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
          => optionbuilder.UseSqlite(@"Data Source=Sample.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
            => modelBuilder.UsePropertyAccessMode(PropertyAccessMode.Property);

        public DbSet<Messenger> Messengers { get; set; }
        public DbSet<ActivityLogEntry> ActivityLog { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
