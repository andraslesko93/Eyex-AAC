using EyexAAC.Model;
using EyexAAC.ViewModel.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel
{
    class UserViewModel : INotifyPropertyChanged
    {
        private static readonly string USERNAME_IS_NULL_MESSAGE = "Please enter a username";
        private static readonly string USERS_TXT_FILE = "username.txt";

        public event PropertyChangedEventHandler PropertyChanged;
        public string UserNameInput { get; set; }
        private static User User { get; set; }
        private string infoMessage;
        public string InfoMessage
        {
            get { return infoMessage; }
            set
            {
                infoMessage = value;
                RaisePropertyChanged("InfoMessage");
            }
        }
        public UserViewModel()
        {
            if (File.Exists(USERS_TXT_FILE))
            { 
                //Set previous username as default value.
                 UserNameInput = File.ReadAllText(USERS_TXT_FILE);
            }
        }

        public bool Login()
        {
            if (string.IsNullOrEmpty (UserNameInput))
            {
                InfoMessage = USERNAME_IS_NULL_MESSAGE;
                return false;
            }
            var user = DatabaseContext.GetUserCredentials(UserNameInput);
            if (user == null)
            {
                user = DatabaseContext.CreateUser(UserNameInput);
            }
            User = user;
            saveUserName(UserNameInput);
            return true;
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public static string GetUsername()
        {
            return User.Username;
        }

        private void saveUserName(string username)
        {
            File.WriteAllText(USERS_TXT_FILE, username);
        }
    }
}
