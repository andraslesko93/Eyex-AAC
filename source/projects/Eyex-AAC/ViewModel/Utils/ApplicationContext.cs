using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel
{
    class ApplicationContext
    {
        public ObservableCollection<Messenger> Messengers { get; set; }

        private static ApplicationContext instance = null;
        private ApplicationContext()
        {
            Messengers = new ObservableCollection<Messenger>();
        }

        public static ApplicationContext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApplicationContext();
                }
                return instance;
            }
        }

        public Messenger GetMessengerFromApplicationContextById(int id)
        {
            var messenger = Messengers.FirstOrDefault(c => c.Id == id);
            return messenger;
        }

        public void RemoveMessenger(Messenger focusedMessenger)
        {
            Messenger focusedMessengerInApplicationContext = Messengers.SingleOrDefault(i => i.Id == focusedMessenger.Id);
            Messengers.Remove(focusedMessengerInApplicationContext);

            if (focusedMessenger.Parent != null)
            {
                Messenger parent = Messengers.SingleOrDefault(i => i.Id == focusedMessenger.Parent.Id);
                if (parent != null)
                {
                    parent.RemoveChild(parent.Children.SingleOrDefault((i => i.Id == focusedMessenger.Id)));
                }
            }
            

        }
    }
}
