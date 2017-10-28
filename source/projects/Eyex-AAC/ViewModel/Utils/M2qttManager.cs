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
        private MqttClient Client { get; set; }
        private string Username { get; set; }
        private string BrokerIpAddress { get; set; }
        private string Password { get; set; }
        public string ClientId { get; set; }
        private SpeechSynthesizer Synthesizer { get; set; }

        public M2qttManager(string brokerIpAddress, string username, string password, SpeechSynthesizer synthesizer, string clientId)
        {
            BrokerIpAddress = brokerIpAddress;
            Username = username;
            Password = password;
            Synthesizer = synthesizer;
            ClientId = clientId;// Guid.NewGuid().ToString();
            Client = new MqttClient(BrokerIpAddress);
            Client.MqttMsgPublishReceived += new MqttClient.MqttMsgPublishEventHandler(EventPublished);
        }
        public void Connect() {
            Thread t = new Thread(EstablishConnection);
            t.Start();
            if (!t.Join(TimeSpan.FromSeconds(5)))
            {
                t.Abort();
                throw new Exception("Unable to connect to the message broker.");
            }
        }
        private void EstablishConnection() { 
            Client.Connect(ClientId, Username, Password);
        }

        public void Publish(string topic, string message)
        {
            MqttMessage mqttMessage = new MqttMessage(ClientId, message);
            string mqttMessageAsJson = JsonConvert.SerializeObject(mqttMessage);
            Client.Publish(topic, Encoding.UTF8.GetBytes(mqttMessageAsJson), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        public void Disconnect()
        {
            if (Client.IsConnected)
            { 
             Client.Disconnect();
            }
        }

        public void Subscribe(string topic)
        {
            if (topic != "")
            {
                Subscribe(new string[] { topic });
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
