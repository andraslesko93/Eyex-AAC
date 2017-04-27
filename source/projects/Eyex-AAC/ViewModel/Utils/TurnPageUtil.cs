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
        private static Boolean _isPreviousPageButtonEnabled;
        private static Boolean _isNextPageButtonEnabled;
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

        public void nextPage(ObservableCollection<MessageMedium> MessageMediums)
        {
            if (IsNextPageButtonEnabled == false)
            {
                return;
            }
            CurrentPageNumber++;
            previousPageButtonStateCalculator();
            nextPageButtonStateCalculator();
            loadMessageMediumsByPageNumber(MessageMediums);
        }

        public void previousPage(ObservableCollection<MessageMedium> MessageMediums)
        {
            if (IsPreviousPageButtonEnabled == false)
            {
                return;
            }
            if (CurrentPageNumber > 1)
            {
                CurrentPageNumber--;
                previousPageButtonStateCalculator();
                nextPageButtonStateCalculator();
                loadMessageMediumsByPageNumber(MessageMediums);
            }
        }


        public void nextPageButtonStateCalculator()
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

        public void previousPageButtonStateCalculator()
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

        public void loadMessageMediumsByPageNumber(ObservableCollection<MessageMedium> MessageMediums)
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

        public void addToMessageCache(MessageMedium messageMedium)
        {
            MessageMediumCache.Add(messageMedium);
        }

    }
}
