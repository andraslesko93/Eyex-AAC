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
                var messengers = context.Messengers.Include(c => c.Children).Where(c => c.Parent == null && (c.Type == MessengerType.general)).ToList();
                return new ObservableCollection<Messenger>(messengers);
            }
        }

        public static ObservableCollection<Messenger> LoadAllTableMessengers()
        {
            using (var context = new MessengerContext())
            {
            var query = from t in context.Messengers where t.Type==MessengerType.general select t;
                var subset = query.ToList().Where(x => x.Parent==null);
                ObservableCollection<Messenger> result = new ObservableCollection<Messenger>(subset);
                return result;
            }
        }


        public static ObservableCollection<Messenger> GetBasicMessengers()
        {
            using (var context = new MessengerContext())
            {
                var messengers = context.Messengers.Where(c => c.Type == MessengerType.pegged).ToList();
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

        public static void DeleteFromDb(Messenger messenger)
        {
            using (var context = new MessengerContext())
            {
                var result = context.Messengers.Include(c => c.Children).Include(c => c.Parent).SingleOrDefault(c => c.Id == messenger.Id);
                if (result != null)
                {
                    List<Messenger> deleteStack = new List<Messenger>();
                    DeleteChildrenFromDB(result.Children, context, deleteStack);
                    foreach (Messenger msg in deleteStack)
                    {
                        context.Messengers.Remove(msg);
                    }
                    if (result.Parent != null)
                    {
                        int parentId = result.Parent.Id;
                        context.Messengers.Remove(result);
                        var parent = context.Messengers.Include(c => c.Children).Include(c => c.Parent).SingleOrDefault(c => c.Id == parentId);
                        if (parent != null && !parent.Children.Any())
                        {
                            parent.HasChild = false;
                        }
                    }
                    else
                    {
                        context.Messengers.Remove(result);
                    }
                    context.SaveChanges();
                }
            }
        }
        private static void DeleteChildrenFromDB(ObservableCollection<Messenger> messageMediumList, MessengerContext context, List<Messenger> deleteStack)
        {
            if (messageMediumList == null)
            {
                return;
            }
            foreach (Messenger msg in messageMediumList)
            {
                var result = context.Messengers.Include(c => c.Children).SingleOrDefault(c => c.Id == msg.Id);
                deleteStack.Add(result);
                DeleteChildrenFromDB(msg.Children, context, deleteStack);
            }
        }

        public static void SaveToDB(Messenger messenger)
        {
            using (var context = new MessengerContext())
            {
                var result = context.Messengers.SingleOrDefault(c => c.Id == messenger.Id);
                if (result != null)
                {
                    result.Name = messenger.Name;
                    result.Image = messenger.Image;
                    context.SaveChanges();
                }
                else
                {
                    var messengerToSave = messenger.Copy();
                    if (messengerToSave.Parent.Type == MessengerType.root)
                    {
                        messengerToSave.Parent = null;
                    }
                    else
                    {
                        //get the parent.
                        var parent = context.Messengers.Include(c => c.Children).SingleOrDefault(c => c.Id == messengerToSave.Parent.Id);
                        parent.AddChild(messengerToSave);
                    }
                    context.Messengers.Add(messengerToSave);
                    context.SaveChanges();
                    messenger.Id = messengerToSave.Id;
                }

            }
        }


    }


}
