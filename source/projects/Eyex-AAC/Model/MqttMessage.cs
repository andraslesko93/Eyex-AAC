using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.Model
{
    class MqttMessage
    {
        public string UserName { get; set; }
        public string MessageBrokerUserName { get; set; }
        public MqttMessageType Type { get; set; }
        public string Payload { get; set; }
        public MqttMessage(string userName, string messageBrokerUserName, string payload, MqttMessageType type)
        {
            UserName = userName;
            MessageBrokerUserName = messageBrokerUserName;
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
