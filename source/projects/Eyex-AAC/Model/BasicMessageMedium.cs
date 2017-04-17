using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.Model
{
    class BasicMessageMedium : MessageMedium
    {
        public BasicMessageMedium() : base() { }
        public BasicMessageMedium(string name, string image) : base(name, image)
        {
            this.Type = "basic";
        }
    } 
}
