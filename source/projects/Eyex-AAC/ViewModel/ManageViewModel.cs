using EyexAAC.Model;
using System.Collections.ObjectModel;
using System.Linq;
using EyexAAC.ViewModel.Utils;
using System.ComponentModel;
using System;
using System.Windows;
using EyexAAC.Common.Utility;

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

        public static ObservableCollection<Messenger> Messengers { get; set; }


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
            GeneralRootMessenger =  new Messenger("Általános üzenethordozók", MessengerType.root);
            PinnedRootMessenger =  new Messenger("Rögzített üzenethordozók", MessengerType.root);
            User = SessionViewModel.User;
            Messengers = new ObservableCollection<Messenger>();
            SetRootObjects();
            FocusedMessenger = null;
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
            foreach (Messenger msg in DatabaseContextUtility.LoadAllGeneralMessenger())
            {
                 GeneralRootMessenger.AddChild(msg);
            }
            if (!Messengers.Contains(GeneralRootMessenger))
            {
                Messengers.Add(GeneralRootMessenger);
            }
            foreach (Messenger msg in DatabaseContextUtility.GetPinnedMessengers())
            {
                PinnedRootMessenger.AddChild(msg);
            }
            if (!Messengers.Contains(PinnedRootMessenger))
            {
                Messengers.Add(PinnedRootMessenger);
            }   
        }

        public void SetMessengerToFocus(Messenger messenger)
        {
            FocusedMessenger = messenger;
            addInProggress = false;
        }
        public bool SaveFocusedMessenger()
        {
            if (IsFocusedMessengerSetted())
            {
                DatabaseContextUtility.SaveMessengerToDB(FocusedMessenger);
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
                SavePinnedMessenger();
            }
        }

        private void SavePinnedMessenger()
        {
            Messenger pinnedMessenger = PinnedMessengerViewModel.PinnedMessengers.SingleOrDefault(c => c.Id == FocusedMessenger.Id);

            if (pinnedMessenger != null)
            {
                pinnedMessenger.Name = FocusedMessenger.Name;
                pinnedMessenger.Image = FocusedMessenger.Image;
            }
            else
            {
                PinnedMessengerViewModel.PinnedMessengers.Add(FocusedMessenger);
            }
        }

        public void LeaveSharingSession()
        {
            string messageBoxText = MessageIds.SHARING_SESSION_SAVE_CONFIRMATION;
            MessageBoxResult result = MessageBox.Show(messageBoxText, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DatabaseContextUtility.SaveMessengers(PageManager.Instance.MessengerCache);
            }
            IsSharingSession = false;
            SetRootObjects();
            PageManager.Instance.NewDataScope(DatabaseContextUtility.LoadAllGeneralMessenger());
            SharingStateMessage = MessageIds.SHARING_SESSION_LEAVE;
        }

        public void ShareMessengers()
        {
            SharingStateMessage = M2qttManager.ShareMessengers();
        }

        public void Connect(string password)
        {
            M2qttManager.initialize(User.MessageBrokerHostName, User.MessageBrokerPort, User.MessageBrokerUsername, password, User.MessageBrokerTopic);
            if (string.IsNullOrEmpty(User.MessageBrokerHostName) || User.MessageBrokerPort==0 || 
                string.IsNullOrEmpty(User.MessageBrokerUsername) || string.IsNullOrEmpty(password) || 
                string.IsNullOrEmpty(User.MessageBrokerTopic)) {
                ConnectionStateMessage = MessageIds.CONNECTION_FIELD_MISSING;
                return;
            }
            ConnectionStateMessage =M2qttManager.Connect();
            M2qttManager.Subscribe();
            //Store credentials for further use.
            DatabaseContextUtility.SaveUserToDB(User);
        }

        public void Disconnect()
        {
            if (M2qttManager != null)
            {
                ConnectionStateMessage = M2qttManager.Disconnect();
            }
        }

        private void SaveGeneralMessengers() {

            PageManager.Instance.NewDataScope(DatabaseContextUtility.LoadAllGeneralMessenger());
            //Temporary disable the saving to application context, and shortcut it with a direct refresh from db.


            /*// Messenger messenger = ApplicationContext.Instance.Messengers.SingleOrDefault(c => c.Id == FocusedMessenger.Id);
            Messenger messenger = ApplicationContext.Instance.findMessengerInTree(FocusedMessenger.Id);

            //Is the element already exists (Edit mode)?
            if (messenger != null)
            {
                messenger.Name = FocusedMessenger.Name;
                messenger.Image = FocusedMessenger.Image;
            }
            //(Create new mode)
            else
            {
                //Should we add the new element to the currently visible elements ?
                if (FocusedMessenger.Parent.Id == PageManager.Instance.ParentMessenger.Id)
                {
                    ApplicationContext.Instance.Messengers.Add(FocusedMessenger);
                    PageManager.Instance.AddToMessengerCache(FocusedMessenger);
                }
                //Add to parent.
                Messenger parentMessenger = ApplicationContext.Instance.Messengers.SingleOrDefault(c => c.Id == FocusedMessenger.Parent.Id);
                if (parentMessenger != null)
                {
                    parentMessenger.AddChild(FocusedMessenger);
                }
            }*/

        }

        public void DeleteFocusedMesageMedium()
        {
            if (FocusedMessenger==null || FocusedMessenger.Type==MessengerType.root)
            {
                return;
            }

            //Remove from database
            DatabaseContextUtility.DeleteFromDb(FocusedMessenger);

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
            PageManager.Instance.NextPageButtonStateCalculator();
        }

        private void DeleteFromApplicationContext()
        {

            if (FocusedMessenger.Type == MessengerType.general)
            {
                ApplicationContext.Instance.RemoveMessenger(FocusedMessenger);
                PageManager.Instance.RemoveMessenger(FocusedMessenger);
                //TODO Reorder in page manager.
            }
            else if (FocusedMessenger.Type == MessengerType.pinned)
            {
                PinnedMessengerViewModel.PinnedMessengers.Remove(PinnedMessengerViewModel.PinnedMessengers.SingleOrDefault(i => i.Id == FocusedMessenger.Id));
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

        public bool IsFocusedMessengerSetted()
        {
            if (FocusedMessenger !=null && FocusedMessenger.Name!=null && FocusedMessenger.Image!=null)
            {
                return true;
            }
            return false;
        }

        public void AddChildToFocusedMessenger()
        {
            if (addInProggress == true || FocusedMessenger.Type==MessengerType.pinned)
            {
                return;
            }
            addInProggress = true;
            Messenger child = new Messenger();
            if (FocusedMessenger == PinnedRootMessenger) {
                child.Type = MessengerType.pinned;
            }
            FocusedMessenger.AddChild(child);
            FocusedMessenger = child;
        }

    }
}
