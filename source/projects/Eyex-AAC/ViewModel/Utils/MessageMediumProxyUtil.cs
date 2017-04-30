using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel.Utils
{
    class MessageMediumProxyUtil
    {
        public static List<MessageMedium> GetTableRootMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.MessageMediums.Include(c => c.Children).Where(c => c.Parent == null && (c.Type == MessageMediumType.table)).ToList();
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

        public static List<MessageMedium> GetBasicMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.MessageMediums.Where(c => c.Type == MessageMediumType.basic).ToList();
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

        public static List<MessageMedium> GetChildren(MessageMedium messageMedium)
        {
            using (var context = new MessageMediumContext())
            {
                var children = context.MessageMediums.Include(c => c.Children).Include(c => c.Parent).Where(c => c.Parent.Id == messageMedium.Id).ToList();
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


    }


}
