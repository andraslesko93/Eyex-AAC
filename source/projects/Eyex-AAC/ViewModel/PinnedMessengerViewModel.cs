using EyexAAC.Model;
using EyexAAC.ViewModel.Utils;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;

namespace EyexAAC.ViewModel
{
    class PinnedMessengerViewModel
    {
        private SpeechSynthesizer synthesizer;
        public SentenceModeManager SentenceModeManager { get; set; }
        public static ObservableCollection<Messenger> PinnedMessengers
        {
            get;
            set;
        }
        public PinnedMessengerViewModel()
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.Volume = 100;
            synthesizer.Rate = -2;
            SentenceModeManager = SentenceModeManager.Instance;
        }

        public void LoadPinnedMessengers()
        {
            PinnedMessengers = new ObservableCollection<Messenger>();
            DatabaseContextUtility.GetPinnedMessengers().ToList().ForEach(PinnedMessengers.Add);
        }
               
        internal void performActionOnPinnedMessengers(int id)
        {
            Messenger pinnedMessenger = GetPinnedMessengersFromCollectionById(id);
            if (SentenceModeManager.SentenceMode)
            {
                SentenceModeManager.AddWord(pinnedMessenger.Name);
            }
            else
            {
                synthesizer.SpeakAsync(pinnedMessenger.Name);
            }
        }
        private Messenger GetPinnedMessengersFromCollectionById(int id)
        {
            var pinnedMessenger = PinnedMessengers.FirstOrDefault(c => c.Id == id);
            return pinnedMessenger;
        }
    }
}

