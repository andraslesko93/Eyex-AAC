using EyexAAC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.ViewModel
{
    class MessageMediumViewModel
    {
        public static List<MessageMedium> messageMediums;

        public MessageMediumViewModel()
        {
           /* messageMediums = new List<MessageMedium>();
            messageMediums.Add(new MessageMedium(Tit));
            messageMediums.Add(new MessageMedium("baby animal", "dummy"));
            messageMediums.Add(new MessageMedium("baby animal", "dummy"));*/
        }

        public static List<MessageMedium> GetMessageMediums()
        {
            return messageMediums;
        }
    }


}
