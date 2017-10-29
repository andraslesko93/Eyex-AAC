using System.ComponentModel.DataAnnotations;

namespace EyexAAC.Model
{
    class User
    {
        [Key]
        public string Username { get; set; }
        public string MessageBrokerUsername { get; set; }
        public string MessageBrokerIpAddress { get; set; }
        public string MessageBrokerPassword { get; set; }

        public User() { }

        public User(string username)
        {
            Username = username;
        }
    }
}
