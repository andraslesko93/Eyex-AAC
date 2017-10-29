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

        public Sentence CurrentSentence { get; set; }

        public string CurrentSentenceAsString
        {
            get { return CurrentSentence.SentenceAsString; }
            set {
                string[] stringArray = value.Split(null);
                CurrentSentence.WordList = stringArray.ToList();
            }

        }

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
            CurrentSentence = new Sentence();
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
            CurrentSentence.AddWord(word);
            RaisePropertyChanged("CurrentSentenceAsString");
        }

        public void PublishSentence(string sentence, string sender)
        {
            SentenceList.Add(new Sentence(sentence, sender));
        }

        public void PublishSentence()
        {
            SentenceList.Add(CurrentSentence);
            CurrentSentence = new Sentence();
            RaisePropertyChanged("CurrentSentenceAsString");
        }

        public class Sentence
        {
            public string Sender { get; set; }
            public string SentenceAsString { get { return string.Join(" ", this.WordList); } }
            public List<String> WordList { get; set; }
            public Sentence() {
                Sender = UserViewModel.GetUsername();
                WordList = new List<string>();
            }

            public Sentence(string sentence, string sender)
            {
                string[] sentenceAsArray = sentence.Split(null);
                WordList = new List<string>(sentenceAsArray);
                Sender = sender;
            }
            public void AddWord(string word)
            {
                WordList.Add(word);
            }
            public void Clear()
            {
                WordList.Clear();
            }
        }
    }
}
