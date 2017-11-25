using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EyexAAC.Model
{
    class User
    {
        [Key]
        public string Username { get; set; }
        public string MessageBrokerUsername { get; set; }
        public string MessageBrokerIpAddress { get; set; }
        public string MessageBrokerTopic { get; set; }
        public string MessageBrokerSubTopic { get; set; }

        public User() { }

        public User(string username)
        {
            Username = username;
        }

        public ICollection<Messenger> Messengers { get; set; }
    }
}
