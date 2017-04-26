using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace EyexAAC.ViewModel
{
    class MessageMediumViewModel : INotifyPropertyChanged
    {
        private static List<MessageMedium> messageMediumsCache;
        private static bool isMetaOpen = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public int MaxRowCount { get; set; }
        public int MaxColumnCount { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public static int CurrentPageNumber { get; set; }
        private static Boolean _isPreviousPageButtonEnabled;
        private static Boolean _isNextPageButtonEnabled;
        public Boolean IsPreviousPageButtonEnabled
        {
            get { return _isPreviousPageButtonEnabled; }
            set
            {
                _isPreviousPageButtonEnabled= value;
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
        public static ObservableCollection<MessageMedium> MessageMediums{ get; set; }
        public MessageMediumViewModel()
        {
            ImageWidth = 193;
            ImageHeight = 163;
            MaxColumnCount = maxColumnCalculator();
            MaxRowCount = maxRowCalculator();
            CurrentPageNumber = 1;

        }
        public void LoadMessageMediums()
        {
            AddInitData();
            MessageMediums = new ObservableCollection<MessageMedium>();
            loadMessageMediumsByPageNumber();
            //Have to call here:
            previousPageButtonStateCalculator();
            nextPageButtonStateCalculator();
        }
        public int AddMessageMediums(MessageMedium messageMedium)
        {
            int returnCode = 0;
            using (var context = new MessageMediumContext())
            {
                context.MessageMediums.Add(messageMedium);
                returnCode = context.SaveChanges();
            }
            if (isMetaOpen == true)
            {
                messageMediumsCache.Add(messageMedium);
            }
            else
            {
                MessageMediums.Add(messageMedium);
            }
            nextPageButtonStateCalculator();
            return returnCode;
        }
        public List<MessageMedium> GetMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.MessageMediums.Where(c => c.IsSubMessage == false && (c.Type == "default" || c.Type=="meta")).ToList();
                foreach (MessageMedium messageMedium in messageMediums)
                {
                    if (messageMedium.ImageAsByte != null)
                    {
                        messageMedium.InitializeImage();
                    }
                }
                return messageMediums;
            }
        }
        internal void performActionOnMessageMedium(int id)
        {
            MessageMedium messageMedium = GetMessageMediumFromCollectionById(id);
            if (messageMedium.Type == "meta")
            {
                messageMediumsCache = new List<MessageMedium>();
                MessageMediums.ToList().ForEach(messageMediumsCache.Add);
                MessageMediums.Clear();

                MessageMedium goBack = new MessageMedium("go back", "pack://application:,,,/Resources/Images/go_back.jpg", "default");
                goBack.Type = "goBack"; //A special goBack MessageMedium to navigate.
                MessageMediums.Add(goBack);

                GetMetaMessageMediumList(messageMedium).ToList().ForEach(MessageMediums.Add);
                isMetaOpen = true;
            }
            else if (messageMedium.Type == "goBack")
            {
                MessageMediums.Clear();
                messageMediumsCache.ToList().ForEach(MessageMediums.Add);
                isMetaOpen = false;
            }
            else
            {
                Console.WriteLine(messageMedium.Name);
                //TODO: Use a reader library instead.
            }
        }

        private MessageMedium GetMessageMediumFromCollectionById(int id)
        {   
            var messageMedium = MessageMediums.FirstOrDefault(c => c.Id == id);
            messageMedium.InitializeImage();
            return messageMedium;   
        }

        /*
        private MessageMedium GetMessageMediumById(int id)
        {
            using (var context = new MessageMediumContext())
            {
                var messageMedium = context.MessageMediums.FirstOrDefault(c => c.Id == id);
                messageMedium.InitializeImage();
                return messageMedium;
            }
        }*/
        private List<MessageMedium> GetMetaMessageMediumList(MessageMedium messageMedium)
        {
            using (var context = new MessageMediumContext())
            {
                var metaMessageMedium = context.MetaMessageMediums.Include(c => c.MessageMediumList).SingleOrDefault(c => c.Id == messageMedium.Id);
                metaMessageMedium.InitializeImages();
                return metaMessageMedium.MessageMediumList;  
                 
            }
        }
        public void AddInitData()
        {
            MessageMedium msg1 = new
               MessageMedium("no", "pack://application:,,,/Resources/Images/no.jpg", "basic");
            MessageMedium msg2 = new
               MessageMedium("yes", "pack://application:,,,/Resources/Images/yes.jpg", "basic");
            MessageMedium msg3 = new
               MessageMedium("n3o", "pack://application:,,,/Resources/Images/newspaper.jpg", "default");
            MessageMedium msg38 = new
               MessageMedium("n3o", "pack://application:,,,/Resources/Images/newspaper.jpg", "default");
            MessageMedium msg4 = new
               MessageMedium("ye5s", "pack://application:,,,/Resources/Images/yes.jpg", "default");
            MetaMessageMedium msg7 = new
              MetaMessageMedium("foods", "pack://application:,,,/Resources/Images/nachos.jpg");
            msg7.AddElement(msg3);
            msg7.AddElement(msg4);


            using (var context = new MessageMediumContext())
            {
                context.MessageMediums.Add(msg1);
                context.MessageMediums.Add(msg2);
                context.MessageMediums.Add(msg38);
                context.MetaMessageMediums.Add(msg7);
                context.SaveChanges();
            }
        }
        private int maxRowCalculator()
        {
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            //Decrease the screenHeight by the bottom row's height
            screenHeight = screenHeight - (ImageHeight + 100);
            return (int)screenHeight / (ImageHeight + 100);
        }
        private int maxColumnCalculator()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            //Decrease by the 2 scroll buttom's width
            screenWidth -= 160;
            double result = screenWidth / (ImageWidth + 80);
            return (int)result;
        }

        private void loadMessageMediumsByPageNumber()
        {
            int maxElementCountOnAPage = MaxColumnCount * MaxRowCount;
            int indexFrom = (CurrentPageNumber-1)*maxElementCountOnAPage;
            int indexTo = CurrentPageNumber * maxElementCountOnAPage;
            MessageMediums.Clear();
            List<MessageMedium> messageMediums = GetMessageMediums();
            for (; (indexFrom < indexTo && indexFrom<messageMediums.Count()); indexFrom++)
            {
                MessageMediums.Add(messageMediums[indexFrom]);
            }
        }

        public void nextPage()
        {
            if (IsNextPageButtonEnabled == false)
            {
                return;
            }
            CurrentPageNumber++;
            previousPageButtonStateCalculator();
            nextPageButtonStateCalculator();
            loadMessageMediumsByPageNumber();
        }

        public void previousPage()
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
                loadMessageMediumsByPageNumber();
            }
        }


        private void nextPageButtonStateCalculator()
        {
            if (GetMessageMediums().Count() > CurrentPageNumber*(MaxColumnCount * MaxRowCount))
            {
                IsNextPageButtonEnabled = true;
            }
            else
            {
                IsNextPageButtonEnabled = false;
            }
        }

        private void previousPageButtonStateCalculator()
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

    }
    
}
