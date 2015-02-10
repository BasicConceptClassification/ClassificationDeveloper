using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class Classifiable
    {
        public ConceptString conceptStr
        {
            get;
            set;
        }

        public Classifier owner
        {
            get;
            set;
        }
    }
}
