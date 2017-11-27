using EyexAAC.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Windows;
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


        public static ObservableCollection<Messenger> SharedMessengers { get;  set; }

        private SynchronizationContext synchronizationContext;
        public void  initialize(string brokerIpAddress, string username, string password)
        {
            synchronizationContext = System.Threading.SynchronizationContext.Current;

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

        public static void Publish(string message)
        {
            MqttMessage mqttMessage = new MqttMessage(ClientId, message, MqttMessageType.SimpleMessage);
            string mqttMessageAsJson = JsonConvert.SerializeObject(mqttMessage);
            Client.Publish(Topic, Encoding.UTF8.GetBytes(mqttMessageAsJson), 1, true);
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

        public string ShareMessengers()
        {
            ObservableCollection<Messenger> messengers = DatabaseContextUtility.LoadAllGeneralMessenger();
            //Serialize payload and create wrapper class.
            MqttMessage mqttMessage = new MqttMessage(ClientId, JsonConvert.SerializeObject(messengers), MqttMessageType.MessengerList);
            //Serialize wrapper class.
            string mqttMessageAsJson = JsonConvert.SerializeObject(mqttMessage);
            try
            {
                Client.Publish(Topic, Encoding.UTF8.GetBytes(mqttMessageAsJson), 0, true);
                return "Your messengers has been shared.";
            }
            catch {
                return "A technical error occured";
            }
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
        private void EventPublished(object sender, MqttMsgPublishEventArgs e)
        {
            string messageAsJson = Encoding.UTF8.GetString(e.Message);
            if (IsValidJson(messageAsJson))
            {
                MqttMessage mqttMessage = JsonConvert.DeserializeObject<MqttMessage>(messageAsJson);
                if (mqttMessage.ClientId == ClientId)
                {
                   return;
                }
                switch (mqttMessage.Type)
                {
                    case MqttMessageType.SimpleMessage:
                        SentenceModeManager.Instance.PublishSentence(mqttMessage.Payload, mqttMessage.ClientId);
                        Synthesizer.SpeakAsync(mqttMessage.ClientId + " say: " + mqttMessage.Payload);
                        Console.WriteLine(mqttMessage.ClientId + mqttMessage.Payload);
                        break;
                    case MqttMessageType.MessengerList:

                        string messageBoxText = mqttMessage.ClientId + " wants to share thier messengers with you, do want to accept them? By accepting them any unsaved changes will be discarded.";
                        MessageBoxResult result = MessageBox.Show(messageBoxText, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                ObservableCollection<Messenger> messengers = DecodePayload(mqttMessage.Payload);
                                SharedMessengers = messengers;
                                ManageViewModel.IsSharingSession = true;
                                synchronizationContext.Post(x => PageManager.Instance.NewDataScope(messengers), null);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.ToString());
                            }
                        }
                        break;
                    default:
                        return;
                }
            }
        }

        private ObservableCollection<Messenger> DecodePayload(string payload)
        {
            ObservableCollection<Messenger> messengers =  JsonConvert.DeserializeObject<ObservableCollection<Messenger>>(payload);
            foreach (Messenger messenger in messengers) {
                if (messenger.HasChild)
                { 
                    foreach (Messenger child in messenger.Children)
                    {
                        child.Parent = messenger;
                    }
                }
            }
            return messengers;
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
