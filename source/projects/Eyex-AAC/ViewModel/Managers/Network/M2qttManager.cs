using EyexAAC.Common.Utility;
using EyexAAC.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
        public static MqttClient Client { get; set; }
        private static MqttSettings settings { get; set; }
        private static string ApplicationUserName { get; set; }

        private byte connectionResponse = 6;

        public static bool IsSubscribed { get; set; } = false;
        private SpeechSynthesizer Synthesizer { get; set; }


        public static ObservableCollection<Messenger> SharedMessengers { get;  set; }

        private SynchronizationContext synchronizationContext;
        public void  initialize(string messageBrokerHostName, uint port, string userName, string password, string topic)
        {
            synchronizationContext = System.Threading.SynchronizationContext.Current;
            settings = new MqttSettings(messageBrokerHostName, port, true, userName, password, topic);
            Synthesizer = new SpeechSynthesizer();
            Synthesizer.Volume = 100;
            try
            {
                Synthesizer.SelectVoice("Microsoft Szabolcs");
            }
            catch (ArgumentException)
            {
                //The choosen voice is not installed, we use the default.
            }
            ApplicationUserName = SessionViewModel.GetUsername();
            Client = new MqttClient(settings.MessageBrokerHostName, Convert.ToInt32(settings.Port), true, null, null, MqttSslProtocols.TLSv1_2);
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
                    responseMessage = MessageIds.CONNECTION_RESPONSE_CONNECTED;
                    break;
                case 1:
                    responseMessage = MessageIds.CONNECTION_RESPONSE_REFUSED_UNACCEPTABLE_PROTOCOL_VERSION;
                    break;
                case 2:
                    responseMessage = MessageIds.CONNECTION_RESPONSE_REFUSED_IDENTIFIER_REJECTED;
                    break;
                case 3:
                    responseMessage = MessageIds.CONNECTION_RESPONSE_REFUSED_SERVER_UNAVAILABLE;
                    break;
                case 4:
                    responseMessage = MessageIds.CONNECTION_RESPONSE_REFUSED_NOT_AUTHORIZED;
                    break;
                case 5:
                    responseMessage = MessageIds.CONNECTION_RESPONSE_REFUSED_BAD_USER_NAME_OR_PASSWORD;
                    break;
                case 6:
                    responseMessage = MessageIds.CONNECTION_RESPONSE_REFUSED_DISCONNECTED;
                    break;
                default:
                    responseMessage = MessageIds.CONNECTION_RESPONSE_UNKNOWN_ERROR;
                    break;
            }
            return responseMessage;
        }

        private void EstablishConnection() {
            try
            {
                connectionResponse = Client.Connect(settings.MessageBrokerUserName, settings.MessageBrokerUserName, settings.Password);
            }
            catch { }
        }

        public static void Publish(string message)
        {
            MqttMessage mqttMessage = new MqttMessage(ApplicationUserName, settings.MessageBrokerUserName ,message, MqttMessageType.SimpleMessage);
            string mqttMessageAsJson = JsonConvert.SerializeObject(mqttMessage);
            Client.Publish(settings.Topic, Encoding.UTF8.GetBytes(mqttMessageAsJson), 1, true);
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
            MqttMessage mqttMessage = new MqttMessage(ApplicationUserName, settings.MessageBrokerUserName, JsonConvert.SerializeObject(messengers), MqttMessageType.MessengerList);
            //Serialize wrapper class.
            string mqttMessageAsJson = JsonConvert.SerializeObject(mqttMessage);
            try
            {
                Client.Publish(settings.Topic, Encoding.UTF8.GetBytes(mqttMessageAsJson), 0, true);
                return MessageIds.MESSENGERS_SHARED;
            }
            catch {
                return MessageIds.TECHNICAL_ERROR;
            }
        }

        public void Subscribe()
        {
            if (Client == null || Client.IsConnected == false)
            {
                return;
            }
            if (settings.Topic != "")
            {
                Subscribe(new string[] { settings.Topic });
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
                if (mqttMessage.MessageBrokerUserName == settings.MessageBrokerUserName)
                {
                   return;
                }
                switch (mqttMessage.Type)
                {
                    case MqttMessageType.SimpleMessage:
                        SentenceModeManager.Instance.PublishSentence(mqttMessage.Payload, mqttMessage.UserName);
                        Synthesizer.SpeakAsync(mqttMessage.UserName + MessageIds.SENDS + mqttMessage.Payload);
                        break;
                    case MqttMessageType.MessengerList:

                        string messageBoxText = mqttMessage.UserName + MessageIds.MESSENGER_SHARING_ASK_FOR_PERMISSION;
                        MessageBoxResult result = MessageBox.Show(messageBoxText, MessageIds.CONFIRMATION, MessageBoxButton.YesNo, MessageBoxImage.Question);
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
        class MqttSettings
        {
            [Required]
            public string MessageBrokerHostName { get; set; }

            [Required]
            public uint Port { get; set; }

            [Required]
            public bool UseSecureConnection { get; set; }

            [Required]
            public string MessageBrokerUserName { get; set; }

            [Required]
            public string Password { get; set; }

            [Required]
            public string Topic { get; set; }

            public MqttSettings(string messageBrokerHostName, uint port, bool useSecureConnection, string messageBrokerUserName, string password, string topic)
            {
                MessageBrokerHostName = messageBrokerHostName;
                Port = port;
                UseSecureConnection = useSecureConnection;
                MessageBrokerUserName = messageBrokerUserName;
                Password = password;
                Topic = topic;
            }
        }
    }
}
