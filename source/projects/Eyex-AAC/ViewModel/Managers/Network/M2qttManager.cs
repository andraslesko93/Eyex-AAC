﻿using EyexAAC.Model;
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
                connectionResponse = Client.Connect(ApplicationUserName, settings.MessageBrokerUserName, settings.Password);
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
                return "Your messengers has been shared.";
            }
            catch {
                return "A technical error occured";
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
                        Synthesizer.SpeakAsync(mqttMessage.UserName + " say: " + mqttMessage.Payload);
                        break;
                    case MqttMessageType.MessengerList:

                        string messageBoxText = mqttMessage.UserName + " wants to share thier messengers with you, do want to accept them? By accepting them any unsaved changes will be discarded.";
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
