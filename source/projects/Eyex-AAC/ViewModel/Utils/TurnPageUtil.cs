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
    class TurnPageUtil : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int MaxRowCount { get; set; }
        public int MaxColumnCount { get; set; }
        public static int CurrentPageNumber { get; set; }
        private Boolean _isPreviousPageButtonEnabled;
        private Boolean _isNextPageButtonEnabled;
        public Boolean IsPreviousPageButtonEnabled
        {
            get { return _isPreviousPageButtonEnabled; }
            set
            {
                _isPreviousPageButtonEnabled = value;
                RaisePropertyChanged("IsPreviousPageButtonEnabled");
            }
        }
        public Boolean IsNextPageButtonEnabled
        {
            get { return _isNextPageButtonEnabled; }
            set
            {
                _isNextPageButtonEnabled = value;
                RaisePropertyChanged("IsNextPageButtonEnabled");
            }
        }
        public List<MessageMedium> MessageMediumCache { get; set; }
        public TurnPageUtil(int maxRowCount, int maxColumnCount, List<MessageMedium> messageMedium)
        {
            MessageMediumCache = messageMedium;
            CurrentPageNumber = 1;
            MaxRowCount = maxRowCount;
            MaxColumnCount = maxColumnCount;
        }

        public void NextPage(ObservableCollection<MessageMedium> MessageMediums)
        {
            if (IsNextPageButtonEnabled == false)
            {
                return;
            }
            CurrentPageNumber++;
            PreviousPageButtonStateCalculator();
            NextPageButtonStateCalculator();
            LoadMessageMediumsByPageNumber(MessageMediums);
        }

        public void PreviousPage(ObservableCollection<MessageMedium> MessageMediums)
        {
            if (IsPreviousPageButtonEnabled == false)
            {
                return;
            }
            if (CurrentPageNumber > 1)
            {
                CurrentPageNumber--;
                PreviousPageButtonStateCalculator();
                NextPageButtonStateCalculator();
                LoadMessageMediumsByPageNumber(MessageMediums);
            }
        }


        public void NextPageButtonStateCalculator()
        {
            if (MessageMediumCache.Count() > CurrentPageNumber * (MaxColumnCount * MaxRowCount))
            {
                IsNextPageButtonEnabled = true;
            }
            else
            {
                IsNextPageButtonEnabled = false;
            }
        }

        public void PreviousPageButtonStateCalculator()
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

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public void LoadMessageMediumsByPageNumber(ObservableCollection<MessageMedium> MessageMediums)
        {
            int maxElementCountOnAPage = MaxColumnCount * MaxRowCount;
            int indexFrom = (CurrentPageNumber - 1) * maxElementCountOnAPage;
            int indexTo = CurrentPageNumber * maxElementCountOnAPage;

            MessageMediums.Clear();
            for (; (indexFrom < indexTo && indexFrom < MessageMediumCache.Count()); indexFrom++)
            {
                MessageMediums.Add(MessageMediumCache[indexFrom]);
            }
        }

        public void AddToMessageCache(MessageMedium messageMedium)
        {
            MessageMediumCache.Add(messageMedium);
        }

        public void logStatus()
        {
            Console.WriteLine("next page state " + IsNextPageButtonEnabled);
            Console.WriteLine("prev page state " + IsPreviousPageButtonEnabled);
            Console.WriteLine("page nr " + CurrentPageNumber);
            Console.WriteLine("element nr" + MessageMediumCache.Count());
        }

        public void NewDataScope(int maxRowCount, int maxColumnCount, List<MessageMedium> messageMediumList)
        {
            MessageMediumCache = messageMediumList;
            MaxRowCount = maxRowCount;
            MaxColumnCount = maxColumnCount;
        }
    }
}
