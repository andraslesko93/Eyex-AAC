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
    class SessionViewModel : INotifyPropertyChanged
    {
        private static readonly string USERNAME_IS_NULL_MESSAGE = "Please enter a username";
        private static readonly string USERS_TXT_FILE = "username.txt";

        public event PropertyChangedEventHandler PropertyChanged;
        public string UserNameInputForLogin { get; set; }
        public static User User { get; set; }
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
        public SessionViewModel()
        {
            if (File.Exists(USERS_TXT_FILE))
            { 
                //Set previous username as default value.
                 UserNameInputForLogin = File.ReadAllText(USERS_TXT_FILE);
            }
        }

        public bool Login()
        {
            if (string.IsNullOrEmpty (UserNameInputForLogin))
            {
                InfoMessage = USERNAME_IS_NULL_MESSAGE;
                return false;
            }
            var user = DatabaseContext.GetUserCredentials(UserNameInputForLogin);
            if (user == null)
            {
                user = DatabaseContext.CreateUser(UserNameInputForLogin);
            }
            User = user;
            saveUserName(UserNameInputForLogin);
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
