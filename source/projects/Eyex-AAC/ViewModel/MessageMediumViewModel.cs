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
        public static ObservableCollection<MessageMedium> MessageMediums{ get; set; }
        public static PageManagerUtil PageManagerUtil { get; set; }
        public static RenderUtil RenderUtil { get; set; }

        public MessageMediumViewModel(){ }
        public void LoadMessageMediums()
        {
            //AddInitData();
            RenderUtil = new RenderUtil();
            PageManagerUtil = new PageManagerUtil(RenderUtil.MaxRowCount, RenderUtil.MaxColumnCount, GetTableRootMessageMediums());
            MessageMediums = new ObservableCollection<MessageMedium>();
            PageManagerUtil.LoadMessageMediumsByPageNumber(MessageMediums);
            PageManagerUtil.PreviousPageButtonStateCalculator();
            PageManagerUtil.NextPageButtonStateCalculator();
        }
        public int AddMessageMediums(MessageMedium messageMedium)
        {
            int returnCode = 0;
            using (var context = new MessageMediumContext())
            {
                context.MessageMediums.Add(messageMedium);
                returnCode = context.SaveChanges();
            }
            MessageMediums.Add(messageMedium);

            PageManagerUtil.AddToMessageCache(messageMedium);
            PageManagerUtil.NextPageButtonStateCalculator();
            return returnCode;
        }
        public List<MessageMedium> GetTableRootMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums= context.MessageMediums.Include(c => c.Children).Where(c => c.Parent == null && (c.Type==MessageMediumType.table)).ToList();
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
            if (messageMedium.Children.Any())
            {
                MoveDownALevel(messageMedium);
            }
            else if (messageMedium.Type == MessageMediumType.goBack)
            {
                MoveUpALevel();
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
        private MessageMedium GetMessageMediumById(int id)
        {
            using (var context = new MessageMediumContext())
            {
                var messageMedium = context.MessageMediums.Include(c => c.Children).Include(c => c.Parent).SingleOrDefault(c => c.Id == id);
                if (messageMedium.ImageAsByte != null)
                {
                    messageMedium.InitializeImage();
                }
                return messageMedium;
            }
        }
        private List<MessageMedium> GetChildren(MessageMedium messageMedium)
        {
            using (var context = new MessageMediumContext())
            {
                var children = context.MessageMediums.Include(c => c.Children).Include(c =>c.Parent).Where(c => c.Parent.Id == messageMedium.Id).ToList();
                foreach (MessageMedium child in children)
                {
                    if (child.ImageAsByte != null)
                    {
                        child.InitializeImage();
                    }
                }
                return children;
            }
        }
        private void AddInitData()
        {
            MessageMedium msg1 = new
               MessageMedium("no", "pack://application:,,,/Resources/Images/no.jpg", MessageMediumType.basic);
            MessageMedium msg2 = new
               MessageMedium("yes", "pack://application:,,,/Resources/Images/yes.jpg", MessageMediumType.basic);
            MessageMedium msg3 = new
               MessageMedium("n3o", "pack://application:,,,/Resources/Images/newspaper.jpg");
            MessageMedium msg5 = new
               MessageMedium("n3o", "pack://application:,,,/Resources/Images/newspaper.jpg");
            MessageMedium msg4 = new
               MessageMedium("ye5s", "pack://application:,,,/Resources/Images/yes.jpg");

            MessageMedium meta1 = new MessageMedium("foods", "pack://application:,,,/Resources/Images/nachos.jpg");

            for (int i = 0; i < 10; i++)
            {
                MessageMedium msg = new MessageMedium(i.ToString(), "pack://application:,,,/Resources/Images/yes.jpg");
                meta1.AddChild(msg);
                for (int j = 0; j< 10; j++)
                {
                    msg.AddChild(new MessageMedium(j.ToString(), "pack://application:,,,/Resources/Images/no.jpg"));
                }
            }
            using (var context = new MessageMediumContext())
            {
                context.MessageMediums.Add(msg1);
                context.MessageMediums.Add(msg2);
                context.MessageMediums.Add(msg3);
                context.MessageMediums.Add(msg4);
                context.MessageMediums.Add(msg5); ;
                context.MessageMediums.Add(meta1);
                context.SaveChanges();
            }
        }
        public void NextPage()
        {
            PageManagerUtil.NextPage(MessageMediums);
        }
        public void PreviousPage()
        {
            PageManagerUtil.PreviousPage(MessageMediums);
        }
        private void MoveDownALevel(MessageMedium messageMedium)
        {
            MessageMediums.Clear();
            setTurnPageUtilScope(addBackMessageMediumToList(GetChildren(messageMedium)));
        }
        private List<MessageMedium> addBackMessageMediumToList(List<MessageMedium> list)
        {
            List<MessageMedium> messageMediumList = new List<MessageMedium>();
            MessageMedium goBack = new MessageMedium("go back", "pack://application:,,,/Resources/Images/go_back.jpg", MessageMediumType.goBack);
            messageMediumList.Add(goBack);
            list.ForEach(messageMediumList.Add);
            return messageMediumList;
        }
        private void MoveUpALevel()
        {
            MessageMedium element = MessageMediums.Last();
            //element.Parent does not contain reference to elements's grandparent, so we have to make another query.
            //The parent object will contain the reference to grandparent.
            MessageMedium parent = GetMessageMediumById(element.Parent.Id);
            MessageMediums.Clear();
            if (parent.Parent != null)
            {
                MessageMedium grandParent = GetMessageMediumById(parent.Parent.Id);
                setTurnPageUtilScope(addBackMessageMediumToList(GetChildren(grandParent)));
            }
            else
            {
                setTurnPageUtilScope(GetTableRootMessageMediums());
            }
        }      
        private void setTurnPageUtilScope(List<MessageMedium> messageMediumList)
        {
            PageManagerUtil.NewDataScope(RenderUtil.MaxRowCount, RenderUtil.MaxColumnCount, messageMediumList);
            PageManagerUtil.LoadMessageMediumsByPageNumber(MessageMediums);
            PageManagerUtil.PreviousPageButtonStateCalculator();
            PageManagerUtil.NextPageButtonStateCalculator();
        }

        public void logStatus()
        {
            PageManagerUtil.logStatus();
        }

    }
}
