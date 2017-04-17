using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel
{
    class AddNewMMViewModel
    {
        public AddNewMMViewModel() { }
        public int addNewMessageMedium(string name, string type, string filePath)
        {
            switch (type)
            {
                case "Family":
                    FamilyMessageMediumViewModel familyMessageMediumViewModel = new FamilyMessageMediumViewModel();
                    return familyMessageMediumViewModel.AddFamilyMessageMediums(new FamilyMessageMedium(name, filePath));
                case "Basic":
                    BasicMessageMediumViewModel basicMessageMediumViewModel = new BasicMessageMediumViewModel();
                    return basicMessageMediumViewModel.AddBasicMessageMediums(new BasicMessageMedium(name, filePath));
                default:
                    MessageMediumViewModel messageMediumViewModel = new MessageMediumViewModel();
                    return messageMediumViewModel.AddMessageMediums(new MessageMedium(name, filePath));
            }
        }
    }
}
