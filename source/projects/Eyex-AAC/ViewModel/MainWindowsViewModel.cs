using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace EyexAAC.ViewModel
{
    class MainWindowsViewModel
    {
        private MessageMediumContext _messageMediumContext = new MessageMediumContext();
        public MainWindowsViewModel()
        {
            //AddInitData();
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

       public List<MessageMedium> GetFamilyMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.MessageMediums.Where(c => c.IsSubMessage == false && c.Type == "family").ToList();
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

        public List<MessageMedium> GetBasicMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.MessageMediums.Where(c => c.IsSubMessage == false && c.Type == "basic").ToList();
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

        public MessageMedium GetMessageMediumById(int id)
        {
            using (var context = new MessageMediumContext())
            {
                var messageMedium = context.MessageMediums.FirstOrDefault(c => c.Id == id);
                messageMedium.InitializeImage();
                return messageMedium;
            }
        }

        public List<MessageMedium> GetMetaMessageMediumList(MessageMedium messageMedium)
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
            using (var context = new MessageMediumContext())
            {
                MessageMedium msg = new MessageMedium("no", "no.jpg");
                context.MessageMediums.Add(msg);
                MetaMessageMedium meta = new MetaMessageMedium("foods", "nachos.jpg");
                meta.AddElement(new MessageMedium("nachos", "nachos.jpg"));
                meta.AddElement(new MessageMedium("paper", "newspaper.jpg"));
                context.MetaMessageMediums.Add(meta);
                context.FamilyMessageMediums.Add(new FamilyMessageMedium("no", "no.jpg"));
                context.FamilyMessageMediums.Add(new FamilyMessageMedium("yes", "yes.jpg"));
                context.FamilyMessageMediums.Add(new FamilyMessageMedium("no", "nachos.jpg"));
                context.BasicMessageMediums.Add(new BasicMessageMedium("no", "no.jpg"));
                context.BasicMessageMediums.Add(new BasicMessageMedium("yes", "yes.jpg"));
                context.BasicMessageMediums.Add(new BasicMessageMedium("no", "nachos.jpg"));
                context.BasicMessageMediums.Add(new BasicMessageMedium("no", "newspaper.jpg"));
                context.SaveChanges();
            }
        }
    }
    
}
