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
    }
}
