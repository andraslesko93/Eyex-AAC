using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace EyexAAC.Model
{
    class MetaMessageMedium : MessageMedium
    {
       // public virtual ICollection<MessageMedium> MessageMediumList { get; set; }
        public List<MessageMedium> MessageMediumList { get; set; }
        public MetaMessageMedium() : base(){}
        public MetaMessageMedium(string name, string image) : base(name, image)
        {
            MessageMediumList = new List<MessageMedium>();
            this.Action = "meta";
            MessageMedium goBack = new MessageMedium("go back", "go_back.jpg");
            goBack.Action = "goBack";
            AddElement(goBack);
        }
        public void AddElement(MessageMedium messageMedium)
        {
            messageMedium.Type = "sub";
            MessageMediumList.Add(messageMedium);
        }
        public void InitializeImages()
        {
            foreach (MessageMedium messageMedium in MessageMediumList)
            {
                messageMedium.InitializeImage();
            }
        }
    }
}
