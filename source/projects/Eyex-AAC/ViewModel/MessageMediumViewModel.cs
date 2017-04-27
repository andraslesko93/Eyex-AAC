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
        public static RenderUtil RenderUtil { get; set; }

        public MessageMediumViewModel(){ }
        public void LoadMessageMediums()
        {
           // AddInitData();
            RenderUtil = new RenderUtil();
            TurnPageUtil = new TurnPageUtil(RenderUtil.MaxRowCount, RenderUtil.MaxColumnCount, GetMessageMediums());
            MessageMediums = new ObservableCollection<MessageMedium>();
            TurnPageUtil.LoadMessageMediumsByPageNumber(MessageMediums);
            TurnPageUtil.PreviousPageButtonStateCalculator();
            TurnPageUtil.NextPageButtonStateCalculator();
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
            TurnPageUtil.AddToMessageCache(messageMedium);
            TurnPageUtil.NextPageButtonStateCalculator();
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
        public void PerformActionOnMessageMedium(int id)
        {
            MessageMedium messageMedium = GetMessageMediumFromCollectionById(id);
            if (messageMedium.Type == "meta")
            {
                OpenMetaMessageMedium(messageMedium);
            }
            else if (messageMedium.Type == "goBack")
            {
                CloseMetaMessageMedium();
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
            MessageMedium msg5 = new
               MessageMedium("n3o", "pack://application:,,,/Resources/Images/newspaper.jpg", "default");
            MessageMedium msg4 = new
               MessageMedium("ye5s", "pack://application:,,,/Resources/Images/yes.jpg", "default");

            MetaMessageMedium meta1 = new
              MetaMessageMedium("foods", "pack://application:,,,/Resources/Images/nachos.jpg");

            for (int i = 0; i < 20; i++) {
                MessageMedium msg = new MessageMedium(i.ToString(), "pack://application:,,,/Resources/Images/yes.jpg", "default");
                meta1.AddElement(msg);
            }
            using (var context = new MessageMediumContext())
            {
                context.MessageMediums.Add(msg1);
                context.MessageMediums.Add(msg2);
                context.MessageMediums.Add(msg3);
                context.MessageMediums.Add(msg4);
                context.MessageMediums.Add(msg5); ;
                context.MetaMessageMediums.Add(meta1);
                context.SaveChanges();
            }
        }
        public void NextPage()
        {
            TurnPageUtil.NextPage(MessageMediums);
        }
        public void PreviousPage()
        {
            TurnPageUtil.PreviousPage(MessageMediums);
        }

        public void OpenMetaMessageMedium(MessageMedium messageMedium)
        {
            messageMediumsCache = new List<MessageMedium>();
            MessageMediums.ToList().ForEach(messageMediumsCache.Add);
            MessageMediums.Clear();

            List<MessageMedium> messageMediumList = new List<MessageMedium>();
            MessageMedium goBack = new MessageMedium("go back", "pack://application:,,,/Resources/Images/go_back.jpg", "default");
            goBack.Type = "goBack";
            messageMediumList.Add(goBack);
            GetMetaMessageMediumList(messageMedium).ToList().ForEach(messageMediumList.Add);
            initNewTurnPageUtil(messageMediumList);
            isMetaOpen = true;
        }

        public void CloseMetaMessageMedium()
        {
            MessageMediums.Clear();
            initNewTurnPageUtil(messageMediumsCache);
            isMetaOpen = false;
        }
        
        
        public void initNewTurnPageUtil(List<MessageMedium> messageMediumList)
        {
            TurnPageUtil = new TurnPageUtil(RenderUtil.MaxRowCount, RenderUtil.MaxColumnCount, messageMediumList);
            TurnPageUtil.LoadMessageMediumsByPageNumber(MessageMediums);
            TurnPageUtil.PreviousPageButtonStateCalculator();
            TurnPageUtil.NextPageButtonStateCalculator();
        }
    }
    
}
