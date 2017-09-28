using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel.Utils
{
    class PageManagerUtil : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int MaxRowCount { get; set; }
        public int MaxColumnCount { get; set; }
        public int CurrentPageNumber { get; set; }
        public int CurrentPageLevel { get; set; }

        public Messenger ParentMessenger { get; set; }
        public ObservableCollection<Messenger> DisplayedMessengers { get; set; }

        public ObservableCollection<Messenger> MessengerCache { get; set; }

        private List<int> PageNumberStack { get; set; }

        private static PageManagerUtil instance = null;

        public static PageManagerUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PageManagerUtil();
                }
                return instance;
            }
        }

        private bool isPreviousPageButtonEnabled;
        private bool isNextPageButtonEnabled;
        private bool isMoveUpButtonEnabled;
        public bool IsPreviousPageButtonEnabled
        {
            get { return isPreviousPageButtonEnabled; }
            set
            {
                isPreviousPageButtonEnabled = value;
                RaisePropertyChanged("IsPreviousPageButtonEnabled");
            }
        }
        public bool IsNextPageButtonEnabled
        {
            get { return isNextPageButtonEnabled; }
            set
            {
                isNextPageButtonEnabled = value;
                RaisePropertyChanged("IsNextPageButtonEnabled");
            }
        }
        public bool IsMoveUpButtonEnabled
        {
            get { return isMoveUpButtonEnabled; }
            set
            {
                isMoveUpButtonEnabled = value;
                RaisePropertyChanged("IsMoveUpButtonEnabled");
            }
        }

        private PageManagerUtil() { }
        public void SetPageManagerUtil(int maxRowCount, int maxColumnCount, ObservableCollection<Messenger> messengers, ObservableCollection<Messenger> displayedMessengers)
        {
            MessengerCache = messengers;
            ParentMessenger = new Messenger();
            CurrentPageLevel = 0;
            CurrentPageNumber = 1;
            PageNumberStack = new List<int>();
            MaxRowCount = maxRowCount;
            MaxColumnCount = maxColumnCount;
            DisplayedMessengers = displayedMessengers;
            LoadMessengers();
        }

        public void NextPage()
        {
            if (IsNextPageButtonEnabled == false)
            {
                return;
            }
            CurrentPageNumber++;
            LoadMessengers();
        }

        public void PreviousPage()
        {
            if (IsPreviousPageButtonEnabled == false)
            {
                return;
            }
            if (CurrentPageNumber > 1)
            {
                CurrentPageNumber--;
                LoadMessengers();
            }
        }

        public void MoveUpALevel()
        {
            if (!IsMoveUpButtonEnabled || CurrentPageLevel == 0 )
            {
                return;
            }
            //Set the current page number and remove it from the stack.
            CurrentPageNumber = PageNumberStack.Last();
            PageNumberStack.RemoveAt(PageNumberStack.Count - 1);

            ObservableCollection<Messenger> newDataScope = new ObservableCollection<Messenger>();
            if (ParentMessenger.Parent != null)
            {
                newDataScope = DatabaseContext.GetChildren(ParentMessenger.Parent);
                if (newDataScope.Any())
                {
                    NewDataScope(newDataScope);
                    ParentMessenger = ParentMessenger.Parent;
                    CurrentPageLevel--;
                }
                else
                {
                    //Parent got deleted
                    CurrentPageLevel = 0;
                    CurrentPageNumber = 1;
                    PageNumberStack.Clear();
                    NewDataScope(DatabaseContext.GetTableRootMessengers());
                }
            }
            else
            {
                //Parent is a root
                ParentMessenger = new Messenger();
                CurrentPageLevel = 0;
                NewDataScope(DatabaseContext.GetTableRootMessengers());
            }
        }

        public void MoveDownALevel(Messenger Parent, ObservableCollection<Messenger> Children)
        {
            ParentMessenger = Parent;
            PageNumberStack.Add(CurrentPageNumber);
            CurrentPageNumber = 1;
            CurrentPageLevel++;
            NewDataScope(Children);
        }

        public void NextPageButtonStateCalculator()
        {
            if (MessengerCache.Count() > CurrentPageNumber * (MaxColumnCount * MaxRowCount))
            {
                IsNextPageButtonEnabled = true;
            }
            else
            {
                IsNextPageButtonEnabled = false;
            }
        }

        private void PreviousPageButtonStateCalculator()
        {
            if (CurrentPageNumber <= 1)
            {
                IsPreviousPageButtonEnabled = false;
            }
            else
            {
                IsPreviousPageButtonEnabled = true;
            }
        }

        public void RemoveMessenger(Messenger focusedMessenger)
        {
            MessengerCache.Remove(MessengerCache.SingleOrDefault(i => i.Id == focusedMessenger.Id));

            if (DisplayedMessengers.Count() == 0)
            {
                if (IsPreviousPageButtonEnabled)
                {
                    PreviousPage();
                    return;
                }
                else if (IsMoveUpButtonEnabled)
                {
                    MoveUpALevel();
                    return;
                }
            }
            
            if (focusedMessenger.Id == ParentMessenger.Id)
            {
                MoveUpALevel();
                return;
            }

            //Move the last element from tha cache to the displayed messengers if needed.
            int maxElementCountOnAPage = MaxColumnCount * MaxRowCount;

            if (DisplayedMessengers.Count() < maxElementCountOnAPage && IsNextPageButtonEnabled)
            {
                int indexTo = CurrentPageNumber * maxElementCountOnAPage;
                DisplayedMessengers.Add(MessengerCache[indexTo - 1]);
            }
            NextPageButtonStateCalculator();
        }

        private void MoveUpButtonStateCalculator()
        {
            if (CurrentPageLevel == 0)
            {
                IsMoveUpButtonEnabled = false;
            }
            else
            {
                IsMoveUpButtonEnabled = true;
            }
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void LoadMessengers()
        {
            int maxElementCountOnAPage = MaxColumnCount * MaxRowCount;
            int indexFrom = (CurrentPageNumber - 1) * maxElementCountOnAPage;
            int indexTo = CurrentPageNumber * maxElementCountOnAPage;

            DisplayedMessengers.Clear();
            for (; (indexFrom < indexTo && indexFrom < MessengerCache.Count()); indexFrom++)
            {
                DisplayedMessengers.Add(MessengerCache[indexFrom]);
            }
            CalculateButtonStates();
        }

        public void AddToMessengerCache(Messenger messenger)
        {
            MessengerCache.Add(messenger);
            NextPageButtonStateCalculator();
        }

        private void NewDataScope(ObservableCollection<Messenger> messengerList)
        {
            MessengerCache = messengerList;
            LoadMessengers();
        }

        private void CalculateButtonStates()
        {
            NextPageButtonStateCalculator();
            PreviousPageButtonStateCalculator();
            MoveUpButtonStateCalculator();
        }
    }
}
