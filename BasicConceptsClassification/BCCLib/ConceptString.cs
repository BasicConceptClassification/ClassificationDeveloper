using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class ConceptString
    {
        /// <summary>
        /// Ordered by how it was classified.
        /// </summary>
        public List<Term> terms
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the string representation of space separated 
        /// terms in the concept string. 
        /// </summary>
        /// <returns>Returns space separated Terms.</returns>
        public override string ToString()
        {
            string result = "";
            foreach (Term t in terms)
            {
                result += t.ToString();
            }
            return result;
        }

        public List<string> ToListstring()
        {
            List<string> results = new List<string>();
            foreach (Term t in terms)
            {
                results.Add(t.ToString());
            }
            results.Reverse();

            return results;
        }
    }
}
