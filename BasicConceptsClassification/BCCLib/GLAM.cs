using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class GLAM
    {
        public GLAM(String _name)
        {
            name = _name;
        }

        public String name
        {
            get;
            set;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
