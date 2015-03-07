using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class GLAM
    {
        public GLAM(String _name, String _url)
        {
            name = _name;
            homeUrl = _url;
        }

        public String name
        {
            get;
            set;
        }

        /// <summary>
        /// Homepage of the GLAM. Can be used as a dafault URL for a Classifiable.
        /// </summary>
        public String homeUrl
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
