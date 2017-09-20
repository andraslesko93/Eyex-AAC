using EyexAAC.Model;
using EyexAAC.ViewModel.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel
{
    class BasicMessageMediumViewModel
    {
        private SpeechSynthesizer synthesizer;
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
        }

        public void LoadBasicMessageMediums()
        {
            BasicMessageMediums = new ObservableCollection<Messenger>();
            DatabaseContext.GetBasicMessengers().ToList().ForEach(BasicMessageMediums.Add);
        }
               
        internal void performActionOnBasicMessageMedium(int id)
        {
            Messenger messageMedium = GetBasicMessageMediumFromCollectionById(id);
            synthesizer.SpeakAsync(messageMedium.Name);
            //Console.WriteLine(messageMedium.Name);
            //TODO: Use a reader library instead. 
        }
        private Messenger GetBasicMessageMediumFromCollectionById(int id)
        {
            var messageMedium = BasicMessageMediums.FirstOrDefault(c => c.Id == id);
            return messageMedium;
        }
    }
}

