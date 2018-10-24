﻿using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;

namespace EyexAAC.ViewModel.Utils
{
    class DatabaseContextUtility
    {
        public static User GetUserCredentials(string username)
        {
            using (var context = new DatabaseContext())
            {
                var user = context.Users.Where(c => c.Username == username).SingleOrDefault();
                return user;
            }
        }

        public static void SaveActivityLog(List<ActivityLogEntry> ActivityLog)
        {
            using (var context = new DatabaseContext())
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
            using (var context = new DatabaseContext())
            {
                User newUser = new User(username);
                context.Users.Add(newUser);
                context.SaveChanges();
                return newUser;   
            }
        }

        public static List<ActivityLogEntry> LoadUnsentActivityLogEntries()
        {
            using (var context = new DatabaseContext())
            {
                var query = from t in context.ActivityLog where t.Status == ActivityLogEntryStatus.unsent select t;
                return query.ToList();
            }
        }
        public static void SetActivityLogEntriesToSent(List<ActivityLogEntry> activitLog)
        {
            using (var context = new DatabaseContext())
            {
                foreach(ActivityLogEntry activityLogEntry in activitLog)
                {
                    var result = context.ActivityLog.SingleOrDefault(c => c.Id == activityLogEntry.Id);
                    if (result != null)
                    {
                        result.Status = ActivityLogEntryStatus.sent;
                    }
                }
                context.SaveChanges();

            }
        }


        public static ObservableCollection<Messenger> LoadAllGeneralMessenger()
        {
            using (var context = new DatabaseContext())
            {
            var query = from t in context.Messengers where t.Type==MessengerType.general && t.Username == SessionViewModel.User.Username select t;
                var subset = query.ToList().Where(x => x.Parent==null);
                ObservableCollection<Messenger> result = new ObservableCollection<Messenger>(subset);

                //// Since visual studio 2015 does not support lazy loading in entity framework it had to set the images manually.
                //foreach (Messenger messenger in result)
                //{
                //    messenger.Image = ByteToBitmapImage(Convert.FromBase64String(messenger.EncodedImage));
                //}
                return result;
            }
        }
        //public static BitmapImage ByteToBitmapImage(byte[] array)
        //{
        //    using (var ms = new System.IO.MemoryStream(array))
        //    {
        //        var image = new BitmapImage();
        //        image.BeginInit();
        //        image.CacheOption = BitmapCacheOption.OnLoad;
        //        image.StreamSource = ms;
        //        image.EndInit();
        //        return image;
        //    }
        //}


        public static ObservableCollection<Messenger> GetPinnedMessengers()
        {
            using (var context = new DatabaseContext())
            {
                var messengers = context.Messengers.Where(c => c.Type == MessengerType.pinned && c.Username == SessionViewModel.User.Username).ToList();
                return new ObservableCollection<Messenger>(messengers);
            }
        }

        public static void DeleteFromDb(Messenger messenger)
        {
            using (var context = new DatabaseContext())
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
        private static void DeleteChildrenFromDB(ObservableCollection<Messenger> messengers, DatabaseContext context, List<Messenger> deleteStack)
        {
            if (messengers == null)
            {
                return;
            }
            foreach (Messenger msg in messengers)
            {
                var result = context.Messengers.Include(c => c.Children).SingleOrDefault(c => c.Id == msg.Id);
                deleteStack.Add(result);
                DeleteChildrenFromDB(msg.Children, context, deleteStack);
            }
        }

        public static void SaveUserToDB(User user)
        {
            using (var context = new DatabaseContext())
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
            using (var context = new DatabaseContext())
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

            using (var context = new DatabaseContext())
            {
                foreach (Messenger messenger in messengers)
                {

                    messenger.Username = SessionViewModel.User.Username;
                    saveMessengerChildren(messenger, context);
                    context.Messengers.Add(messenger);
                }
                context.SaveChanges();
            }
        }

        private static void saveMessengerChildren(Messenger messenger, DatabaseContext context)
        {
            if (messenger.HasChild) {
                foreach (Messenger child in messenger.Children)
                {
                    child.Username = SessionViewModel.User.Username;
                    context.Messengers.Add(child);
                    saveMessengerChildren(child, context);
                }
            }
        }



    }


}
