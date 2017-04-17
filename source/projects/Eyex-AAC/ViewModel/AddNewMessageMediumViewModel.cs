using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel
{
    class AddNewMessageMediumViewModel
    {
        private MessageMediumContext _messageMediumContext = new MessageMediumContext();
        private MessageMediumViewModel messageMediumViewModel = new MessageMediumViewModel();
        public int addNewMessageMedium(string name, string type, string filePath)
        {            
                switch (type)
                {
                    case "Family":
                        FamilyMessageMedium familyMessageMedium = new FamilyMessageMedium(name, filePath);
                        return messageMediumViewModel.AddFamilyMessageMediums(familyMessageMedium);
                    case "Basic":
                        BasicMessageMedium basicMessageMedium = new BasicMessageMedium(name, filePath);
                        return messageMediumViewModel.AddBasicMessageMediums(basicMessageMedium);
                    default:
                        MessageMedium messageMedium = new MessageMedium(name, filePath);
                        return messageMediumViewModel.AddMessageMediums(messageMedium);
                }         
        }
    }
}
