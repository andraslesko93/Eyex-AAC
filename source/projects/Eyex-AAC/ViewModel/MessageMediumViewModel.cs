using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace EyexAAC.ViewModel
{
    class MessageMediumViewModel
    {
        public int MaxRowCount { get; set; }
        public int MaxColumnCount { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        private static List<MessageMedium> messageMediumsCache;
        private static bool isMetaOpen = false;
        public static ObservableCollection<MessageMedium> MessageMediums
        {
            get;
            set;
        }
        public MessageMediumViewModel()
        {

            ImageWidth = 193;
            ImageHeight = 163; 
            MaxColumnCount = maxColumnCalculator();
            MaxRowCount = maxRowCalculator();
        }
        public void LoadMessageMediums()
        {
           // AddInitData();
            MessageMediums = new ObservableCollection<MessageMedium>();
            GetMessageMediums().ToList().ForEach(MessageMediums.Add);
        }
        public int AddMessageMediums(MessageMedium messageMedium)
        {
            int returnCode = 0;
            using (var context = new MessageMediumContext())
            {
                context.MessageMediums.Add(messageMedium);
                returnCode = context.SaveChanges();
            }
            if (isMetaOpen == true)
            {
                messageMediumsCache.Add(messageMedium);
            }
            else
            {
                MessageMediums.Add(messageMedium);
            }
            return returnCode;
        }
        public List<MessageMedium> GetMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.MessageMediums.Where(c => c.IsSubMessage == false && (c.Type == "default" || c.Type=="meta")).ToList();
                foreach (MessageMedium messageMedium in messageMediums)
                {
                    if (messageMedium.ImageAsByte != null)
                    {
                        messageMedium.InitializeImage();
                    }
                }
                return messageMediums;
            }
        }
        internal void performActionOnMessageMedium(int id)
        {
            MessageMedium messageMedium = GetMessageMediumFromCollectionById(id);
            if (messageMedium.Type == "meta")
            {
                messageMediumsCache = new List<MessageMedium>();
                MessageMediums.ToList().ForEach(messageMediumsCache.Add);
                MessageMediums.Clear();

                MessageMedium goBack = new MessageMedium("go back", "pack://application:,,,/Resources/Images/go_back.jpg");
                goBack.Type = "goBack"; //A special goBack MessageMedium to navigate.
                MessageMediums.Add(goBack);

                GetMetaMessageMediumList(messageMedium).ToList().ForEach(MessageMediums.Add);
                isMetaOpen = true;
            }
            else if (messageMedium.Type == "goBack")
            {
                MessageMediums.Clear();
                messageMediumsCache.ToList().ForEach(MessageMediums.Add);
                isMetaOpen = false;
            }
            else
            {
                Console.WriteLine(messageMedium.Name);
                //TODO: Use a reader library instead.
            }
        }

        private MessageMedium GetMessageMediumFromCollectionById(int id)
        {   
            var messageMedium = MessageMediums.FirstOrDefault(c => c.Id == id);
            messageMedium.InitializeImage();
            return messageMedium;   
        }

        /*
        private MessageMedium GetMessageMediumById(int id)
        {
            using (var context = new MessageMediumContext())
            {
                var messageMedium = context.MessageMediums.FirstOrDefault(c => c.Id == id);
                messageMedium.InitializeImage();
                return messageMedium;
            }
        }*/
        private List<MessageMedium> GetMetaMessageMediumList(MessageMedium messageMedium)
        {
            using (var context = new MessageMediumContext())
            {
                var metaMessageMedium = context.MetaMessageMediums.Include(c => c.MessageMediumList).SingleOrDefault(c => c.Id == messageMedium.Id);
                metaMessageMedium.InitializeImages();
                return metaMessageMedium.MessageMediumList;  
                 
            }
        }
        public void AddInitData()
        {
            BasicMessageMedium msg1 = new
                BasicMessageMedium("no", "pack://application:,,,/Resources/Images/no.jpg");
            BasicMessageMedium msg2 = new
                BasicMessageMedium("yes", "pack://application:,,,/Resources/Images/yes.jpg");
            MessageMedium msg3 = new
               MessageMedium("n3o", "pack://application:,,,/Resources/Images/newspaper.jpg");
            MessageMedium msg38 = new
               MessageMedium("n3o", "pack://application:,,,/Resources/Images/newspaper.jpg");
            MessageMedium msg4 = new
                MessageMedium("ye5s", "pack://application:,,,/Resources/Images/yes.jpg");
            FamilyMessageMedium msg5 = new
               FamilyMessageMedium("no", "pack://application:,,,/Resources/Images/no.jpg");
            FamilyMessageMedium msg6 = new
                FamilyMessageMedium("yes", "pack://application:,,,/Resources/Images/yes.jpg");
            FamilyMessageMedium msg9 = new
               FamilyMessageMedium("yes", "pack://application:,,,/Resources/Images/nachos.jpg");
            MetaMessageMedium msg7 = new
              MetaMessageMedium("foods", "pack://application:,,,/Resources/Images/nachos.jpg");
            msg7.AddElement(msg3);
            msg7.AddElement(msg4);


            using (var context = new MessageMediumContext())
            {
                context.BasicMessageMediums.Add(msg1);
                context.BasicMessageMediums.Add(msg2);
                context.MessageMediums.Add(msg38);
                context.FamilyMessageMediums.Add(msg5);
                context.FamilyMessageMediums.Add(msg6);
                context.FamilyMessageMediums.Add(msg9);
                context.MetaMessageMediums.Add(msg7);
                context.SaveChanges();
            }
        }
        private int maxRowCalculator()
        {
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            //Decrease the screenHeight by the bottom row's height
            screenHeight = screenHeight - (ImageHeight + 100);
            return (int)screenHeight / (ImageHeight + 100);
        }
        private int maxColumnCalculator()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            //Decrease by the 2 scroll buttom's width
            screenWidth -= 160;
            double result = screenWidth / (ImageWidth + 80);
            return (int)result;
        }
    }
    
}
