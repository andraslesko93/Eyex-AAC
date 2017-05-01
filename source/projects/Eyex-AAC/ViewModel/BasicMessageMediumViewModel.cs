using EyexAAC.Model;
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
        public static ObservableCollection<MessageMedium> BasicMessageMediums
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
            BasicMessageMediums = new ObservableCollection<MessageMedium>();
            GetBasicMessageMediums().ToList().ForEach(BasicMessageMediums.Add);
        }
        
        public int AddBasicMessageMediums(MessageMedium basicMessageMedium)
        {
            int returnCode = 0;
            using (var context = new MessageMediumContext())
            {
                context.MessageMediums.Add(basicMessageMedium);
                returnCode = context.SaveChanges();
            }
            BasicMessageMediums.Add(basicMessageMedium);
            return returnCode;
        }

        public List<MessageMedium> GetBasicMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.MessageMediums.Where(c => c.Type == MessageMediumType.basic).ToList();
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
       
        internal void performActionOnBasicMessageMedium(int id)
        {
            MessageMedium messageMedium = GetBasicMessageMediumFromCollectionById(id);
            synthesizer.SpeakAsync(messageMedium.Name);
            //Console.WriteLine(messageMedium.Name);
            //TODO: Use a reader library instead. 
        }
        private MessageMedium GetBasicMessageMediumFromCollectionById(int id)
        {
            var messageMedium = BasicMessageMediums.FirstOrDefault(c => c.Id == id);
            messageMedium.InitializeImage();
            return messageMedium;
        }
    }
}

