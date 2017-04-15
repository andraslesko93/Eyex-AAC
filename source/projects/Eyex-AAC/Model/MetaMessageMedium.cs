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
        public List<MessageMedium> MessageMediumList { get; set; }
        public MetaMessageMedium() : base() { }
        public MetaMessageMedium(string name, BitmapImage image) : base(name, image)
        {
            MessageMediumList = new List<MessageMedium>();
            this.IsItMeta = true;
        }
        public void AddElement(MessageMedium messageMedium)
        {
            messageMedium.Type = "sub";
            MessageMediumList.Add(messageMedium);
        }
    }
}
