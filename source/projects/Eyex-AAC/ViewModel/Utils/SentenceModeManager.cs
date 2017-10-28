using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EyexAAC.ViewModel.Utils
{
    class SentenceModeManager: INotifyPropertyChanged
    {
        public MTObservableCollection<Sentence> SentenceList { get; set; }

        private static bool isSentenceActive = false;
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
            SentenceList = new MTObservableCollection<Sentence>();
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

        public void AddWord(string word) {
            if (isSentenceActive == false)
            {
                NewSentence();
                isSentenceActive = true;
            }
            SentenceList.Last().AddWord(word);
        }

        public List<String> GetLastSentence() { return SentenceList.Last().WordList; }

        public class Sentence: INotifyPropertyChanged
        {
            public string Sender { get; set; }
            public string SentenceAsString { get { return string.Join(" ", this.WordList); } }
            public List<String> WordList { get; set; }
            public Sentence() {
                Sender = MessengerViewModel.CLIENT_ID;
                WordList = new List<string>();
            }

            public Sentence(string sentence, string sender)
            {
                string[] sentenceAsArray = sentence.Split(null);
                WordList = new List<string>(sentenceAsArray);
                Sender = sender;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void AddWord(string word)
            {
                WordList.Add(word);
                RaisePropertyChanged("SentenceAsString");
            }
            private void RaisePropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }

        public void NewSentence()
        {
            SentenceList.Add(new Sentence());
        }

        public void NewSentence(string sentence, string sender)
        {
            SentenceList.Add(new Sentence(sentence, sender));
            EndSentence();
        }

        public void EndSentence()
        {
            isSentenceActive = false;
        }
    }
}
