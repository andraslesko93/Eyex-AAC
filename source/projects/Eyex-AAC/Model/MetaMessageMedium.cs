﻿using System;
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
            Type = "meta";
            MessageMediumList = new List<MessageMedium>();
            /*MessageMedium goBack = new MessageMedium("go back", "pack://application:,,,/Resources/Images/go_back.jpg");
            goBack.Type = "goBack"; //A special goBack MessageMedium to navigate.
            AddElement(goBack);*/
        }
        public void AddElement(MessageMedium messageMedium)
        {
            messageMedium.IsSubMessage = true;
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
