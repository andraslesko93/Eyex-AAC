using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace EyexAAC.ViewModel.Utils
{
    class DatabaseContext
    {
       public static ObservableCollection<Messenger> GetTableRootMessengers()
        {
            using (var context = new MessengerContext())
            {
                var messengers = context.Messengers.Include(c => c.Children).Where(c => c.Parent == null && (c.Type == MessengerType.table)).ToList();
                return new ObservableCollection<Messenger>(messengers);
            }
        }

        public static ObservableCollection<Messenger> LoadAllTableMessengers()
        {
            using (var context = new MessengerContext())
            {
            var query = from t in context.Messengers where t.Type==MessengerType.table select t;
                var subset = query.ToList().Where(x => x.Parent==null);
                ObservableCollection<Messenger> result = new ObservableCollection<Messenger>(subset);
                return result;
            }
        }


        public static ObservableCollection<Messenger> GetBasicMessengers()
        {
            using (var context = new MessengerContext())
            {
                var messengers = context.Messengers.Where(c => c.Type == MessengerType.basic).ToList();
                return new ObservableCollection<Messenger>(messengers);
            }
        }

        public static ObservableCollection<Messenger> GetChildren(Messenger messenger)
        {
            using (var context = new MessengerContext())
            {
                var children = context.Messengers.Include(c => c.Children).Include(c => c.Parent).Where(c => c.Parent.Id == messenger.Id).ToList();
                return new ObservableCollection<Messenger>(children);
            }
        }


    }


}
