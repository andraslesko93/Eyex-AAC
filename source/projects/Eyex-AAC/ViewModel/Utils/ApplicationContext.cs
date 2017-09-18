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
        public ObservableCollection<MessageMedium> Messengers { get; set; }

        private static ApplicationContext instance = null;
        private ApplicationContext()
        {
            Messengers = new ObservableCollection<MessageMedium>();
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

        public MessageMedium GetMessageMediumFromApplicationContextById(int id)
        {
            var messenger = Messengers.FirstOrDefault(c => c.Id == id);
            messenger.InitializeImage();
            return messenger;
        }
    }
}
