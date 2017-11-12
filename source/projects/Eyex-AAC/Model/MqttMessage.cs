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
        public MqttMessageType Type { get; set; }
        public string Payload { get; set; }
        public MqttMessage(string clientId, string payload, MqttMessageType type)
        {
            ClientId = clientId;
            Payload = payload;
            Type = type;
        }
    }
    enum MqttMessageType
    {
        SimpleMessage,
        MessengerList
    }
}
