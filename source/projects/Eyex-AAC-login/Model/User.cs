using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
namespace EyexAAC.Model
{
    class User { 
        public string username { get; set; }

        public string password { get; set; }

        public bool isLoggedIn { get; set; }

        public User(string username, string password) {
            this.username = username;
            this.password = password;
        }
        public User() { }
        public static void AddUser(User user)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=Database.sqlite;Version=3;");
            connection.Open();
            try
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO user VALUES (@username, @password)";
                    command.Parameters.Add(new SQLiteParameter("@username", user.username));
                    command.Parameters.Add(new SQLiteParameter("@password", user.password));
                    command.ExecuteNonQuery();
                } 
            }
            catch (Exception)
            {
                throw;
            }
            connection.Close();
        }

        public static User GetUser(string username)
        {

            SQLiteConnection connection = new SQLiteConnection("Data Source=Database.sqlite;Version=3;");
            connection.Open();
            User user = new User();
            try
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"select username, password from user where username=@username";
                    command.Parameters.Add(new SQLiteParameter("@username") { Value = username });
                    command.CommandType = System.Data.CommandType.Text;

                    SQLiteDataReader reader;
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        user.username = reader["username"].ToString();
                        user.password = reader["password"].ToString();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return user;
        }

    }
}
