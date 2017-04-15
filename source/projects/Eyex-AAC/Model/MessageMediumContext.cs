using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.Model
{
    class MessageMediumContext : DbContext
    {
        public DbSet<MessageMedium> MessageMediums { get; set; }
    }
}
