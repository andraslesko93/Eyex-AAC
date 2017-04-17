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

namespace EyexAAC.ViewModel
{
    class MessageMediumViewModel
    {
        private List<MessageMedium> messageMediumsCache;
        public static ObservableCollection<MessageMedium> MessageMediums
        {
            get;
            set;
        }
        public static ObservableCollection<FamilyMessageMedium> FamilyMessageMediums
        {
            get;
            set;
        }
        public static ObservableCollection<BasicMessageMedium> BasicMessageMediums
        {
            get;
            set;
        }

        public void LoadMessageMediums()
        {
           // AddInitData();

            MessageMediums = new ObservableCollection<MessageMedium>();
            GetMessageMediums().ToList().ForEach(MessageMediums.Add);

            FamilyMessageMediums = new ObservableCollection<FamilyMessageMedium>();
            GetFamilyMessageMediums().ToList().ForEach(FamilyMessageMediums.Add);

            BasicMessageMediums = new ObservableCollection<BasicMessageMedium>();
            GetBasicMessageMediums().ToList().ForEach(BasicMessageMediums.Add);
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
            return returnCode;
        }

        public int AddFamilyMessageMediums(FamilyMessageMedium familyMessageMedium)
        {
            int returnCode = 0;
            using (var context = new MessageMediumContext())
            {
                context.FamilyMessageMediums.Add(familyMessageMedium);
                returnCode = context.SaveChanges();
            }
            FamilyMessageMediums.Add(familyMessageMedium);
            return returnCode;
        }

        public int AddBasicMessageMediums(BasicMessageMedium basicMessageMedium)
        {
            int returnCode = 0;
            using (var context = new MessageMediumContext())
            {
                context.BasicMessageMediums.Add(basicMessageMedium);
                returnCode = context.SaveChanges();
            }
            BasicMessageMediums.Add(basicMessageMedium);
            return returnCode;
        }

        private MessageMediumContext _messageMediumContext = new MessageMediumContext();
        public MessageMediumViewModel()
        {

        }
       public List<MessageMedium> GetMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.MessageMediums.Where(c => c.IsSubMessage == false && c.Type == "default").ToList();
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
       public List<FamilyMessageMedium> GetFamilyMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.FamilyMessageMediums.Where(c => c.IsSubMessage == false).ToList();
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

        public List<BasicMessageMedium> GetBasicMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.BasicMessageMediums.Where(c => c.IsSubMessage == false).ToList();
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
            MessageMedium messageMedium = GetMessageMediumById(id);
            if (messageMedium.Action == "meta")
            {
                messageMediumsCache = new List<MessageMedium>();
                MessageMediums.ToList().ForEach(messageMediumsCache.Add);
                MessageMediums.Clear();
                GetMetaMessageMediumList(messageMedium).ToList().ForEach(MessageMediums.Add);
            }
            else if (messageMedium.Action == "goBack")
            {
                MessageMediums.Clear();
                messageMediumsCache.ToList().ForEach(MessageMediums.Add);
            }
            else
            {
                Console.WriteLine(messageMedium.Name);
                //TODO: Use a reader library instead.
            }
        }

        private MessageMedium GetMessageMediumById(int id)
        {
            using (var context = new MessageMediumContext())
            {
                var messageMedium = context.MessageMediums.FirstOrDefault(c => c.Id == id);
                messageMedium.InitializeImage();
                return messageMedium;
            }
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
            BasicMessageMedium msg1 = new
                BasicMessageMedium("no", "pack://application:,,,/Resources/Images/no.jpg");
            BasicMessageMedium msg2 = new
                BasicMessageMedium("yes", "pack://application:,,,/Resources/Images/yes.jpg");

            MessageMedium msg3 = new
               MessageMedium("no", "pack://application:,,,/Resources/Images/no.jpg");
            MessageMedium msg4 = new
                MessageMedium("yes", "pack://application:,,,/Resources/Images/yes.jpg");

            FamilyMessageMedium msg5 = new
               FamilyMessageMedium("no", "pack://application:,,,/Resources/Images/no.jpg");
            FamilyMessageMedium msg6 = new
                FamilyMessageMedium("yes", "pack://application:,,,/Resources/Images/yes.jpg");

            MetaMessageMedium msg7 = new
              MetaMessageMedium("foods", "pack://application:,,,/Resources/Images/nachos.jpg");
            msg7.AddElement(msg3);
            msg7.AddElement(msg4);


            using (var context = new MessageMediumContext())
            {
                context.BasicMessageMediums.Add(msg1);
                context.BasicMessageMediums.Add(msg2);
                context.MessageMediums.Add(msg3);
                context.MessageMediums.Add(msg4);
                context.FamilyMessageMediums.Add(msg5);
                context.FamilyMessageMediums.Add(msg6);
                context.MetaMessageMediums.Add(msg7);
                context.SaveChanges();
            }
        }
    }
    
}
