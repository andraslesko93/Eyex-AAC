using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel
{
    class FamilyMessageMediumViewModel
    {
        public static ObservableCollection<FamilyMessageMedium> FamilyMessageMediums
        {
            get;
            set;
        }
        public FamilyMessageMediumViewModel() { }

        public void LoadFamilyMessageMediums()
        {
            FamilyMessageMediums = new ObservableCollection<FamilyMessageMedium>();
            GetFamilyMessageMediums().ToList().ForEach(FamilyMessageMediums.Add);
        }

        public int AddFamilyMessageMediums(FamilyMessageMedium familyMessageMedium)
        {
            int returnCode = 0;
            using (var context = new MessageMediumContext())
            {
                context.FamilyMessageMediums.Add(familyMessageMedium);
                returnCode = context.SaveChanges();
            }
            FamilyMessageMediums.Add(familyMessageMedium);
            return returnCode;
        }

        public List<FamilyMessageMedium> GetFamilyMessageMediums()
        {
            using (var context = new MessageMediumContext())
            {
                var messageMediums = context.FamilyMessageMediums.Where(c => c.IsSubMessage == false).ToList();
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
        internal void performActionOnFamilyMessageMedium(int id)
        {
            FamilyMessageMedium messageMedium = GetFamilyMessageMediumFromCollectionById(id);
            Console.WriteLine(messageMedium.Name);
            //TODO: Use a reader library instead. 
        }

        private FamilyMessageMedium GetFamilyMessageMediumFromCollectionById(int id)
        {
            var messageMedium = FamilyMessageMediums.FirstOrDefault(c => c.Id == id);
            messageMedium.InitializeImage();
            return messageMedium;
        }
    }
}

