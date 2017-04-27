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
using EyexAAC.ViewModel.Utils;

namespace EyexAAC.ViewModel
{
    class MessageMediumViewModel
    {
        private static List<MessageMedium> messageMediumsCache;
        private static bool isMetaOpen = false;

       
        public static ObservableCollection<MessageMedium> MessageMediums{ get; set; }
        public static TurnPageUtil TurnPageUtil { get; set; }

        public RenderUtil RenderUtil { get; set; }
        public MessageMediumViewModel()
        {
            
        }
        public void LoadMessageMediums()
        {
            AddInitData();
            RenderUtil = new RenderUtil();
            TurnPageUtil = new TurnPageUtil(RenderUtil.MaxRowCount, RenderUtil.MaxColumnCount, GetMessageMediums());
            MessageMediums = new ObservableCollection<MessageMedium>();
            TurnPageUtil.loadMessageMediumsByPageNumber(MessageMediums);
            //Have to call here:
            TurnPageUtil.previousPageButtonStateCalculator();
            TurnPageUtil.nextPageButtonStateCalculator();
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
            TurnPageUtil.addToMessageCache(messageMedium);
            TurnPageUtil.nextPageButtonStateCalculator();
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

                MessageMedium goBack = new MessageMedium("go back", "pack://application:,,,/Resources/Images/go_back.jpg", "default");
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
            MessageMedium msg1 = new
               MessageMedium("no", "pack://application:,,,/Resources/Images/no.jpg", "basic");
            MessageMedium msg2 = new
               MessageMedium("yes", "pack://application:,,,/Resources/Images/yes.jpg", "basic");
            MessageMedium msg3 = new
               MessageMedium("n3o", "pack://application:,,,/Resources/Images/newspaper.jpg", "default");
            MessageMedium msg38 = new
               MessageMedium("n3o", "pack://application:,,,/Resources/Images/newspaper.jpg", "default");
            MessageMedium msg4 = new
               MessageMedium("ye5s", "pack://application:,,,/Resources/Images/yes.jpg", "default");
            MetaMessageMedium msg7 = new
              MetaMessageMedium("foods", "pack://application:,,,/Resources/Images/nachos.jpg");
            msg7.AddElement(msg3);
            msg7.AddElement(msg4);


            using (var context = new MessageMediumContext())
            {
                context.MessageMediums.Add(msg1);
                context.MessageMediums.Add(msg2);
                context.MessageMediums.Add(msg38);
                context.MetaMessageMediums.Add(msg7);
                context.SaveChanges();
            }
        }
        public void nextPage()
        {
            TurnPageUtil.nextPage(MessageMediums);
        }

        public void previousPage()
        {
            TurnPageUtil.previousPage(MessageMediums);
        }

    }
    
}
