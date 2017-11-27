using System.Data.Entity;

namespace EyexAAC.Model
{
    class ActivityLogContext: DbContext
    {
        public DbSet<ActivityLogEntry> ActivityLog { get; set; }
        
    }
}
