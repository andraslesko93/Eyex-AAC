using EyexAAC.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace EyexAAC.ViewModel.Utils
{
    class M2qttManager
    {
        private static readonly string TOPIC_SEPARATOR = "/";
        public static MqttClient Client { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private static string ClientId { get; set; }

        private byte connectionResponse = 6;

        public static bool IsSubscribed { get; set; } = false;
        public static string Topic { get; set; }
        private SpeechSynthesizer Synthesizer { get; set; }

        public void  initialize(string brokerIpAddress, string username, string password)
        {
            Username = username;
            Password = password;
            Synthesizer = new SpeechSynthesizer();
            Synthesizer.Volume = 100;
            Synthesizer.Rate = -2; ;
            ClientId = SessionViewModel.GetUsername();
            Client = new MqttClient(brokerIpAddress);
            Client.MqttMsgPublishReceived += new MqttClient.MqttMsgPublishEventHandler(EventPublished);
        }
        public string Connect() {
            if (Client.IsConnected)
            {
                return null;
            }
            Thread t = new Thread(EstablishConnection);
            t.IsBackground = true;
            t.Start();
            if (!t.Join(TimeSpan.FromSeconds(5)))
            {
                t.Abort();
                connectionResponse = 3;
            }
            return GetConnectionResponseMessage();
        }

        public string GetConnectionResponseMessage()
        {
            string responseMessage;
            switch(connectionResponse)
            {
                case 0:
                    responseMessage = "Connected";
                    break;
                case 1:
                    responseMessage = "Connection Refused, unacceptable protocol version";
                    break;
                case 2:
                    responseMessage = "Connection Refused, identifier rejected";
                    break;
                case 3:
                    responseMessage = "Connection Refused, Server unavailable";
                    break;
                case 4:
                    responseMessage = "Connection Refused, bad user name or password";
                    break;
                case 5:
                    responseMessage = "Connection Refused, not authorized";
                    break;
                case 6:
                    responseMessage = "Disconnected";
                    break;
                default:
                    responseMessage = "Unknown error occured.";
                    break;
            }
            return responseMessage;
        }

        private void EstablishConnection() {
            try
            {
                connectionResponse = Client.Connect(ClientId, Username, Password);
            }
            catch { }
        }

        public static void Publish(string topic, string message)
        {
            MqttMessage mqttMessage = new MqttMessage(ClientId, message);
            string mqttMessageAsJson = JsonConvert.SerializeObject(mqttMessage);
            Client.Publish(topic, Encoding.UTF8.GetBytes(mqttMessageAsJson), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        public string Disconnect()
        {
            if (Client != null &&Client.IsConnected)
            { 
                Client.Disconnect();
                connectionResponse = 6;
                return GetConnectionResponseMessage();
            }
            return "";
        }

        public void Subscribe(string topic, string subtopic)
        {
            if (Client == null || Client.IsConnected == false)
            {
                return;
            }
            Topic = topic + TOPIC_SEPARATOR + subtopic;
            if (Topic != "")
            {
                Subscribe(new string[] { Topic });
                IsSubscribed = true;
            }
        }

        public void Subscribe(string[] topics)
        {
            if (topics.Length !=0)
            {
                Client.Subscribe(topics, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
        }
        private void EventPublished(Object sender, MqttMsgPublishEventArgs e)
        {
            string messageAsJson = System.Text.UTF8Encoding.UTF8.GetString(e.Message);
            if (IsValidJson(messageAsJson))
            { 
                MqttMessage mqttMessage = JsonConvert.DeserializeObject<MqttMessage>(messageAsJson);
                if (mqttMessage.ClientId != ClientId) {
                    SentenceModeManager.Instance.PublishSentence(mqttMessage.Message, mqttMessage.ClientId);
                    Synthesizer.SpeakAsync(mqttMessage.ClientId+" say: "+ mqttMessage.Message);
                    Console.WriteLine(mqttMessage.ClientId + mqttMessage.Message);
                }
            }

            
        }

        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
