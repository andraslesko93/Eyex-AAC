using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel
{
    class BasicMessageMediumViewModel
    {
        public static ObservableCollection<BasicMessageMedium> BasicMessageMediums
        {
            get;
            set;
        }
        public BasicMessageMediumViewModel() { }

        public void LoadBasicMessageMediums()
        {
            BasicMessageMediums = new ObservableCollection<BasicMessageMedium>();
            GetBasicMessageMediums().ToList().ForEach(BasicMessageMediums.Add);
        }
        
        public int AddBasicMessageMediums(BasicMessageMedium basicMessageMedium)
        {
            int returnCode = 0;
            using (var context = new MessageMediumContext())
            {
                context.BasicMessageMediums.Add(basicMessageMedium);
                returnCode = context.SaveChanges();
            }
            BasicMessageMediums.Add(basicMessageMedium);
            return returnCode;
        }

        public List<BasicMessageMedium> GetBasicMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.BasicMessageMediums.Where(c => c.IsSubMessage == false).ToList();
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
            BasicMessageMedium messageMedium = GetBasicMessageMediumFromCollectionById(id);
            Console.WriteLine(messageMedium.Name);
            //TODO: Use a reader library instead. 
        }
        private BasicMessageMedium GetBasicMessageMediumFromCollectionById(int id)
        {
            var messageMedium = BasicMessageMediums.FirstOrDefault(c => c.Id == id);
            messageMedium.InitializeImage();
            return messageMedium;
        }
    }
}

