using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class Term
    {
        public String rawTerm
        {
            get;
            set;
        }

        public List<Term> subTerms
        {
            get;
            set;
        }
    }
}
