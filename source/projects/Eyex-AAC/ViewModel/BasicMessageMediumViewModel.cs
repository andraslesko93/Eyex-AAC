using EyexAAC.Model;
using EyexAAC.ViewModel.Utils;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;

namespace EyexAAC.ViewModel
{
    class BasicMessageMediumViewModel
    {
        private SpeechSynthesizer synthesizer;
        public SentenceModeManager SentenceModeManager { get; set; }
        public static ObservableCollection<Messenger> BasicMessageMediums
        {
            get;
            set;
        }
        public BasicMessageMediumViewModel()
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.Volume = 100;
            synthesizer.Rate = -2;
            SentenceModeManager = SentenceModeManager.Instance;
        }

        public void LoadBasicMessageMediums()
        {
            BasicMessageMediums = new ObservableCollection<Messenger>();
            DatabaseContextUtility.GetBasicMessengers().ToList().ForEach(BasicMessageMediums.Add);
        }
               
        internal void performActionOnBasicMessageMedium(int id)
        {
            Messenger messageMedium = GetBasicMessageMediumFromCollectionById(id);
            if (SentenceModeManager.SentenceMode)
            {
                SentenceModeManager.AddWord(messageMedium.Name);
            }
            else
            {
                synthesizer.SpeakAsync(messageMedium.Name);
            }
        }
        private Messenger GetBasicMessageMediumFromCollectionById(int id)
        {
            var messageMedium = BasicMessageMediums.FirstOrDefault(c => c.Id == id);
            return messageMedium;
        }
    }
}

