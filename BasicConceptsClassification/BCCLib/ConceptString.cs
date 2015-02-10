using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class ConceptString
    {
        public List<Term> terms
        {
            get;
            set;
        }

        public override string ToString()
        {
            string result = "";
            foreach (Term t in terms)
            {
                result += t.ToString();
                result += " ";
            }
            return result;
        }
    }
}
