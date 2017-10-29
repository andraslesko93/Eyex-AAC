using System.Data.Entity;

namespace EyexAAC.Model
{
    class MessengerContext : DbContext
    {
        public DbSet<Messenger> Messengers { get; set; }
    }
}
