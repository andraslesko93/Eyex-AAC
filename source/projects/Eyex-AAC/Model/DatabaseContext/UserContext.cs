using System.Data.Entity;

namespace EyexAAC.Model
{
    class UserContext: DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
