﻿using EyexAAC.Model;
using System.Collections.ObjectModel;
using System.Linq;
using EyexAAC.ViewModel.Utils;
using System.ComponentModel;
using System;
using System.Windows;

namespace EyexAAC.ViewModel
{
    class ManageViewModel : INotifyPropertyChanged
    {
        private static Messenger _focusedMessenger;

        private static Messenger GeneralRootMessenger;
        private static Messenger PinnedRootMessenger;
        private bool addInProggress = false;

        public User User { get; set; }
        public static M2qttManager M2qttManager { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static string connectionStateMessage;
        public string ConnectionStateMessage
        {
            get { return connectionStateMessage; }
            set
            {
                connectionStateMessage = value;
                RaisePropertyChanged("ConnectionStateMessage");
            }
        }

        private static string sharingStateMessage;
        public static string SharingStateMessage
        {
            get { return sharingStateMessage; }
            set
            {
                sharingStateMessage = value;
                RaiseStaticPropertyChanged("SharingStateMessage");
            }
        }

        private static bool isSharingSession;
        public static bool IsSharingSession
        {
            get { return isSharingSession; }
            set
            {
                isSharingSession = value;
                RaiseStaticPropertyChanged("IsSharingSession");

                if (value == true) {
                    SharingStateMessage = "You are now in a sharing session.";
                }
            }
        }

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
            GeneralRootMessenger =  new Messenger("General messengers", MessengerType.root);
            PinnedRootMessenger =  new Messenger("Pinned messengers", MessengerType.root);
            User = SessionViewModel.User;
            MessageMediums = new ObservableCollection<Messenger>();
            SetRootObjects();

            M2qttManager = new M2qttManager();
            if (string.IsNullOrEmpty(ConnectionStateMessage))
            {
                ConnectionStateMessage = M2qttManager.GetConnectionResponseMessage();
            }
        }

        private void SetRootObjects()
        {
            if (IsSharingSession)
            {
                return;
            }
            foreach (Messenger msg in DatabaseContext.LoadAllGeneralMessenger())
            {
                 GeneralRootMessenger.AddChild(msg);
            }
            if (!MessageMediums.Contains(GeneralRootMessenger))
            {
                MessageMediums.Add(GeneralRootMessenger);
            }
            foreach (Messenger msg in DatabaseContext.GetBasicMessengers())
            {
                PinnedRootMessenger.AddChild(msg);
            }
            if (!MessageMediums.Contains(PinnedRootMessenger))
            {
                MessageMediums.Add(PinnedRootMessenger);
            }   
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
                DatabaseContext.SaveMessengerToDB(FocusedMessenger);
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
            else if (FocusedMessenger.Type == MessengerType.pinned)
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

        public void LeaveSharingSession()
        {
            string messageBoxText = "If you leave sharing session, all shared messengers will be discarded, do you want to save them?";
            MessageBoxResult result = MessageBox.Show(messageBoxText, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DatabaseContext.SaveMessengers(PageManagerUtil.Instance.MessengerCache);
            }
            IsSharingSession = false;
            SetRootObjects();
            PageManagerUtil.Instance.NewDataScope(DatabaseContext.LoadAllGeneralMessenger());
            SharingStateMessage = "You have left the sharing session.";
        }

        public void ShareMessengers()
        {
            SharingStateMessage = M2qttManager.ShareMessengers();
        }

        public void Connect(string password)
        {
            M2qttManager.initialize(User.MessageBrokerIpAddress, User.MessageBrokerUsername, password);
            ConnectionStateMessage =M2qttManager.Connect();
            M2qttManager.Subscribe(User.MessageBrokerTopic, User.MessageBrokerSubTopic);

            //Store credentials for further use.
            DatabaseContext.SaveUserToDB(User);
        }

        public void Disconnect()
        {
            if (M2qttManager != null)
            {
                ConnectionStateMessage = M2qttManager.Disconnect();
            }
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
            else if (FocusedMessenger.Type == MessengerType.pinned)
            {
                BasicMessageMediumViewModel.BasicMessageMediums.Remove(BasicMessageMediumViewModel.BasicMessageMediums.SingleOrDefault(i => i.Id == FocusedMessenger.Id));
            }
        }
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private static void RaiseStaticPropertyChanged(string property)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(property));
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
