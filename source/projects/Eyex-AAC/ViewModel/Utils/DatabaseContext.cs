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
        public static User GetUserCredentials(string username)
        {
            using (var context = new UserContext())
            {
                var user = context.Users.Where(c => c.Username == username).SingleOrDefault();
                return user;
            }
        }

        public static void SaveActivityLog(List<ActivityLogEntry> ActivityLog)
        {
            using (var context = new ActivityLogContext())
            {
                foreach (ActivityLogEntry activityLogEntry in ActivityLog)
                {
                    context.ActivityLog.Add(activityLogEntry);
                }
                context.SaveChanges();
            }
        }

        public static User CreateUser(string username)
        {
            using (var context = new UserContext())
            {
                User newUser = new User(username);
                context.Users.Add(newUser);
                context.SaveChanges();
                return newUser;   
            }
        }

        public static List<ActivityLogEntry> LoadUnsentActivityLogEntries()
        {
            using (var context = new ActivityLogContext())
            {
                var query = from t in context.ActivityLog where t.Status == ActivityLogEntryStatus.unsent select t;
                return query.ToList();
            }
        }

        public static ObservableCollection<Messenger> LoadAllGeneralMessenger()
        {
            using (var context = new MessengerContext())
            {
            var query = from t in context.Messengers where t.Type==MessengerType.general && t.Username == SessionViewModel.User.Username select t;
                var subset = query.ToList().Where(x => x.Parent==null);
                ObservableCollection<Messenger> result = new ObservableCollection<Messenger>(subset);
                return result;
            }
        }


        public static ObservableCollection<Messenger> GetBasicMessengers()
        {
            using (var context = new MessengerContext())
            {
                var messengers = context.Messengers.Where(c => c.Type == MessengerType.pinned && c.Username == SessionViewModel.User.Username).ToList();
                return new ObservableCollection<Messenger>(messengers);
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

        public static void SaveUserToDB(User user)
        {
            using (var context = new UserContext())
            {
                var result = context.Users.SingleOrDefault(c => c.Username == user.Username);
                if (result != null)
                {
                    result.MessageBrokerUsername = user.MessageBrokerUsername;
                    result.MessageBrokerIpAddress = user.MessageBrokerIpAddress;
                    result.MessageBrokerTopic = user.MessageBrokerTopic;
                    result.MessageBrokerSubTopic = user.MessageBrokerSubTopic;
                    context.SaveChanges();
                }
            }
        }

        public static void SaveMessengerToDB(Messenger messenger)
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
                    messengerToSave.Username = SessionViewModel.User.Username;
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

        public static void SaveMessengers(ObservableCollection<Messenger> messengers)
        {

            using (var context = new MessengerContext())
            {
                foreach (Messenger messenger in messengers)
                {
                    messenger.Username = SessionViewModel.User.Username;
                    context.Messengers.Add(messenger);
                }
                context.SaveChanges();
            }
        }



    }


}
