using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace EyexAAC.ViewModel.Utils
{
    class M2qttManager
    {
        private static M2qttManager instance = null;
        public static M2qttManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new M2qttManager();
                }
                return instance;
            }
        }
        private static MqttClient client;
        private static readonly string BROKER_IP_ADDRESS = "192.168.0.230";
        private static readonly string USERNAME = "user2";
        private static readonly string PASSWORD = "password";

        private M2qttManager()
        {
            client = new MqttClient(BROKER_IP_ADDRESS);
            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId, USERNAME, PASSWORD);
        }


        public void Publish(string topic, string message)
        {
            client.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        /*public static void anyad()
        {
            MqttClient clientSub;
            clientSub = new MqttClient("192.168.0.230");
            string clientId = Guid.NewGuid().ToString();
            clientSub.Connect(clientId, "user2", "password");
            Console.WriteLine("-------------------" + clientSub.IsConnected);
            clientSub.Publish("dev/test", Encoding.UTF8.GetBytes("eyexcsa"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }*/


        MqttClient Client { get; set; }
        string ClientId { get; set; }
        string TxtTopicSubscribe { get; set; }
        string TxtReceived { get; set; }
        string TxtTopicPublish { get; set; }
        string TxtPublish { get; set; }


        // this code runs when the main window opens (start of the app)
       /* public M2qttManager()
        {
            //InitializeComponent();

            string BrokerAddress = "192.168.0.230";

           // Client = new MqttClient(BrokerAddress, 1883);

            // register a callback-function (we have to implement, see below) which is called by the library when a message was received
            Client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            // use a unique id as client id, each time we start the application
            ClientId = "fake";// Guid.NewGuid().ToString();
            byte code = Client.Connect(Guid.NewGuid().ToString(), "user2", "password");
            Client.Connect(ClientId);

            // whole topic
            //string Topic = "/ElektorMyJourneyIoT/" + TxtTopicPublish + "/test";

            // publish a message with QoS 2
            Client.Publish("dev/test", Encoding.UTF8.GetBytes("hello eyex"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }*/


        // this code runs when the main window closes (end of the app)
        protected void Close()
        {
            Client.Disconnect();
        }


        // this code runs when the button "Subscribe" is clicked
        private void Subscribe()
        {
            if (TxtTopicSubscribe != "")
            {
                // whole topic
                string Topic = "/ElektorMyJourneyIoT/" + TxtTopicSubscribe + "/test";

                // subscribe to the topic with QoS 2
                Client.Subscribe(new string[] { Topic }, new byte[] { 2 });   // we need arrays as parameters because we can subscribe to different topics with one call
                TxtReceived = "";
            }
            else
            {
                System.Windows.MessageBox.Show("You have to enter a topic to subscribe!");
            }
        }


        // this code runs when the button "Publish" is clicked
        private void Publish()
        {
            if (TxtTopicPublish != "")
            {
                // whole topic
                string Topic = "/ElektorMyJourneyIoT/" + TxtTopicPublish + "/test";

                // publish a message with QoS 2
                Client.Publish(Topic, Encoding.UTF8.GetBytes(TxtPublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }
            else
            {
                System.Windows.MessageBox.Show("You have to enter a topic to publish!");
            }
        }


        // this code runs when a message was received
        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
           /* Dispatcher.Invoke(delegate {              // we need this construction because the receiving code in the library and the UI with textbox run on different threads
                txtReceived = ReceivedMessage;
            });*/
        }
    }
}
