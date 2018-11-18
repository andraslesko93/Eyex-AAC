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
       /* //Recursive search to return a children if it's exist in the treestructure of the application context.
        public Messenger findMessengerInTree(int id) {
            return findMessenger(Messengers, id);
        }

        private Messenger findMessenger(ObservableCollection<Messenger> messengers, int id) {
            Messenger result = messengers.SingleOrDefault(c => c.Id == id);
            if (result == null) {
                foreach (Messenger messenger in messengers)
                {
                    if (messenger.HasChild) {
                        return findMessenger(messenger.Children, id);
                    }
                }
            }
            return result;
        }*/
    }
}
