using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyexAAC.Model;
namespace EyexAAC.ViewModel
{
    class Authentication
    {
        public static bool isAuthenticated;
        public static User currentUser;
        public static bool Login(string username, string password)
        {
            User matchingUser = User.GetUser(username);
            if (matchingUser.password == password)
            {
                Console.WriteLine("logged in");
                currentUser = matchingUser;
                isAuthenticated = true;
                return true;
            }
            Console.WriteLine("wrong pass");
            return false;
        }

        public static bool Registrate(string username, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return false;
            }
            User user = new User(username, password);
            User.AddUser(user);
            return true;
        }
    }
}
