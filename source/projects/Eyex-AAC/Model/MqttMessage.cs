using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.Model
{
    class MqttMessage
    {
        public string ClientId { get; set; }
        public string Message { get; set; }
        public MqttMessage(string clientId, string message)
        {
            ClientId = clientId;
            Message = message;
        }
    }
}
