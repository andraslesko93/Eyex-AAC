using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyexAAC.Model
{
    class FamilyMessageMedium : MessageMedium
    {
        public FamilyMessageMedium() : base() { }
        public FamilyMessageMedium(string name, string image) : base(name, image)
        {
            this.Type = "family";
        }
    }
}
