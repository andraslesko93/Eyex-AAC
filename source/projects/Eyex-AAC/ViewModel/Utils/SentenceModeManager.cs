using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel.Utils
{
    class SentenceModeManager: INotifyPropertyChanged
    {
        public ObservableCollection<String> WordList { get; set; }

        private bool sentenceMode;
        public bool SentenceMode
        {
            get { return sentenceMode; }
            set
            {
                sentenceMode = value;
                RaisePropertyChanged("SentenceMode");
            }
        }

        private static SentenceModeManager instance = null;
        public event PropertyChangedEventHandler PropertyChanged;

        private SentenceModeManager()
        {
            WordList = new ObservableCollection<String>();
            SentenceMode = false;
        }

        public static SentenceModeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SentenceModeManager();
                }
                return instance;
            }
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }


    }
}
