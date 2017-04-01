using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace EyexAAC.Model
{
    class ParentModel
    {
        public static SQLiteConnection connection;
        public ParentModel()
        {
            connection = new SQLiteConnection("Data Source=Database.sqlite;Version=3;");
            connection.Open();
        }
    }
}
