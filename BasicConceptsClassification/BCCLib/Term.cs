using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    /// <summary>
    /// A Term is used in the classification scheme to classify Classifiables.
    /// <para>Terms are used in ConceptStrings and in the BCC.</para>
    /// </summary>
    public class Term
    {
        // Terms currently have an "id" which was their Protege IRI. (Think URL.)
        // Should switch to GUIDs eventually?
        public string id
        {
            get;
            set;
        }
        /// <summary>
        /// The string content of the Term.
        /// </summary>
        public String rawTerm
        {
            get;
            set;
        }

        /// <summary>
        /// A list of children of the Term.
        /// </summary>
        public List<Term> subTerms
        {
            get;
            set;
        }
    }
}
