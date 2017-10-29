﻿using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using EyexAAC.ViewModel.Utils;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.ComponentModel;
using System.Security;

namespace EyexAAC.ViewModel
{
    class ManageViewModel : INotifyPropertyChanged
    {
        private static Messenger _focusedMessenger;

        private static Messenger TableRoot;
        private static Messenger BasicRoot;
        private bool addInProggress = false;

        public User User { get; set; }
        public M2qttManager M2qttManager { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public static ObservableCollection<Messenger> MessageMediums { get; set; }

        public Messenger FocusedMessenger
        {
            get { return _focusedMessenger; }
            set
            {
                _focusedMessenger = value;
                RaisePropertyChanged("FocusedMessenger");
            }
        }

        public ManageViewModel()
        {
            TableRoot =  new Messenger("General messengers", MessengerType.root);
            BasicRoot =  new Messenger("Pegged messengers", MessengerType.root);
            User = SessionViewModel.User;
            MessageMediums = new ObservableCollection<Messenger>();
            SetRootObjects();
        }

        private void SetRootObjects()
        {       
            foreach (Messenger msg in DatabaseContext.LoadAllTableMessengers())
            {
                 TableRoot.AddChild(msg);
            }
            MessageMediums.Add(TableRoot);

            foreach (Messenger msg in DatabaseContext.GetBasicMessengers())
            {
                BasicRoot.AddChild(msg);
            }
            MessageMediums.Add(BasicRoot);
        }

        public void SetMessageMediumToFocus(Messenger messageMedium)
        {
            FocusedMessenger = messageMedium;
            addInProggress = false;
        }
        public bool SaveFocusedMessenger()
        {
            if (IsFocusMessageMediumSetted())
            {
                DatabaseContext.SaveToDB(FocusedMessenger);
                SaveToApplicationContext();
                return true;
            }
            addInProggress = false;
            return false;
        }

        private void SaveToApplicationContext()
        {
            if (FocusedMessenger.Type == MessengerType.general)
            {
                SaveGeneralMessengers();
            }
            else if (FocusedMessenger.Type == MessengerType.pegged)
            {
                SavePeggedMessenger();
            }
        }

        private void SavePeggedMessenger()
        {
            Messenger basicMessageMedium = BasicMessageMediumViewModel.BasicMessageMediums.SingleOrDefault(c => c.Id == FocusedMessenger.Id);

            if (basicMessageMedium != null)
            {
                basicMessageMedium.Name = FocusedMessenger.Name;
                basicMessageMedium.Image = FocusedMessenger.Image;
            }
            else
            {
                BasicMessageMediumViewModel.BasicMessageMediums.Add(FocusedMessenger);
            }
        }

        internal void Connect(SecureString password)
        {

            M2qttManager = new M2qttManager(User.MessageBrokerIpAddress, User.MessageBrokerUsername, "password");
            M2qttManager.Connect();
            M2qttManager.Subscribe("dev/test");

            //Connect and store credentials...
        }

        private void SaveGeneralMessengers() {
            Messenger messenger = ApplicationContext.Instance.Messengers.SingleOrDefault(c => c.Id == FocusedMessenger.Id);
            //Is the element already in the currently visible elements (Edit mode)?
            if (messenger != null)
            {
                messenger.Name = FocusedMessenger.Name;
                messenger.Image = FocusedMessenger.Image;
            }
            //(Create new mode)
            else
            {
                //Should we add the new element to the currently visible elements ?
                if (FocusedMessenger.Parent.Id == PageManagerUtil.Instance.ParentMessenger.Id)
                {
                    ApplicationContext.Instance.Messengers.Add(FocusedMessenger);
                    PageManagerUtil.Instance.AddToMessengerCache(FocusedMessenger);
                }
            }

            //Add to parent.
            Messenger parentMessenger = ApplicationContext.Instance.Messengers.SingleOrDefault(c => c.Id == FocusedMessenger.Parent.Id);
            if (parentMessenger != null)
            {
                parentMessenger.AddChild(FocusedMessenger);
            }
        }

        public void DeleteFocusedMesageMedium()
        {
            if (FocusedMessenger==null || FocusedMessenger.Type==MessengerType.root)
            {
                return;
            }

            //Remove from database
            DatabaseContext.DeleteFromDb(FocusedMessenger);

            //Remove from application context
            DeleteFromApplicationContext();

            //Remove from treeview
            Messenger parent = FocusedMessenger.Parent;
            parent.RemoveChild(FocusedMessenger);

            //Set parent to focus
            FocusedMessenger = parent;

            //Reset the state of add in progress
            addInProggress = false;

            //Recalculate next page button
            PageManagerUtil.Instance.NextPageButtonStateCalculator();
        }

        private void DeleteFromApplicationContext()
        {

            if (FocusedMessenger.Type == MessengerType.general)
            {
                ApplicationContext.Instance.RemoveMessenger(FocusedMessenger);
                PageManagerUtil.Instance.RemoveMessenger(FocusedMessenger);
                //TODO Reorder in page manager.
            }
            else if (FocusedMessenger.Type == MessengerType.pegged)
            {
                BasicMessageMediumViewModel.BasicMessageMediums.Remove(BasicMessageMediumViewModel.BasicMessageMediums.SingleOrDefault(i => i.Id == FocusedMessenger.Id));
            }
        }
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public bool IsFocusMessageMediumSetted()
        {
            if (FocusedMessenger !=null && FocusedMessenger.Name!=null && FocusedMessenger.Image!=null)
            {
                return true;
            }
            return false;
        }

        public void AddChildToFocusedMessenger()
        {
            if (addInProggress == true)
            {
                return;
            }
            addInProggress = true;
            Messenger child = new Messenger();
            FocusedMessenger.AddChild(child);
            FocusedMessenger = child;
        }

    }
}
