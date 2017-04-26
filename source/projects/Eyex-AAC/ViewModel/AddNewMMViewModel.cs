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
                case "Basic":
                    BasicMessageMediumViewModel basicMessageMediumViewModel = new BasicMessageMediumViewModel();
                    return basicMessageMediumViewModel.AddBasicMessageMediums(new MessageMedium(name, filePath, "basic"));
                default:
                    MessageMediumViewModel messageMediumViewModel = new MessageMediumViewModel();
                    return messageMediumViewModel.AddMessageMediums(new MessageMedium(name, filePath, "default"));
            }
        }
    }
}
