using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel.Utils
{
    class DatabaseContext
    {
        public static List<Messenger> GetTableRootMessengers()
        {
            using (var context = new MessengerContext())
            {
                var messengers = context.Messengers.Include(c => c.Children).Where(c => c.Parent == null && (c.Type == MessengerType.table)).ToList();
                foreach (Messenger messenger in messengers)
                {
                    if (messenger.ImageAsByte != null)
                    {
                        messenger.InitializeImage();
                    }
                }
                return messengers;
            }
        }

        public static List<Messenger> GetBasicMessengers()
        {
            using (var context = new MessengerContext())
            {
                var messengers = context.Messengers.Where(c => c.Type == MessengerType.basic).ToList();
                foreach (Messenger messenger in messengers)
                {
                    if (messenger.ImageAsByte != null)
                    {
                        messenger.InitializeImage();
                    }
                }
                return messengers;
            }
        }

        public static List<Messenger> GetChildren(Messenger messenger)
        {
            using (var context = new MessengerContext())
            {
                var children = context.Messengers.Include(c => c.Children).Include(c => c.Parent).Where(c => c.Parent.Id == messenger.Id).ToList();
                foreach (Messenger child in children)
                {
                    if (child.ImageAsByte != null)
                    {
                        child.InitializeImage();
                    }
                }
                return children;
            }
        }

        /*public static Messenger GetMessengerById(int id)
        {
            using (var context = new MessengerContext())
            {
                var messenger = context.Messengers.Include(c => c.Children).Include(c => c.Parent).SingleOrDefault(c => c.Id == id);
                if (messenger.ImageAsByte != null)
                {
                    messenger.InitializeImage();
                }
                return messenger;
            }
        }*/


    }


}
