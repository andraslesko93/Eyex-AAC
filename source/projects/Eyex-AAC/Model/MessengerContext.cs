using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.Model
{
    class MessengerContext : DbContext
    {
        public DbSet<Messenger> Messengers { get; set; }
    }
}
