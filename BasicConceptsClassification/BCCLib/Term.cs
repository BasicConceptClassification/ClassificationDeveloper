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
        /// <summary>Terms currently have an "id" which was their Protege IRI. 
        /// (Think URL.) Should switch to GUIDs eventually?</summary> 
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
        /// The lower case version of rawTerm.
        /// </summary>
        public String lower
        {
            get;
            set;
        }

        /// <summary>
        /// A list of sub Terms (children) of the Term.
        /// </summary>
        public List<Term> subTerms
        {
            get;
            set;
        }

        /// <summary>
        /// ToString() format: (rawTerm)
        /// </summary>
        /// <returns>Parenthesis of rawTerm.</returns>
        public override string ToString()
        {
            return "(" + rawTerm + ")";
        }

        /// <summary>
        /// Checks if the Term has the subTerm t. Compares by rawTerm. 
        /// Returns index in the subTerms.
        /// </summary>
        /// <param name="t">Term to compare by.</param>
        /// <returns>Index of the subTerm in the subTerm List, -1 if does not exist.</returns>
        public int hasSubTerm(Term t)
        {
            return subTerms.FindIndex(0, subTerms.Count, a => a.rawTerm == t.rawTerm);
        }

        /// <summary>
        /// Creates all terms along the list of terms provided, order from immediate
        /// subTerm to furthest subterm. The current term should NOT be included in 
        /// the list of terms. The list of terms passed in is NOT modified.
        /// </summary>
        /// <param name="tList">List of terms starting with the Term's immediate subterm
        /// that need to be added to the Term's subTerms.</param>
        public void connectTermsFromList(List<Term> trmList)
        {
            // Check if we're at the end condition.
            // If so, check if the term has a list of subterms existing.
            // If it doesn't, create and be done.
            if (trmList.Count == 0)
            {
                if (subTerms == null)
                {
                    subTerms = new List<Term>();
                }
                return;
            }

            // Otherwise, we still have some child term to add or more paths
            // to traverse down.
            int nextIndex = this.hasSubTerm(trmList[0]);

            // If some subterm exists, we can just traverse down it.
            if (nextIndex >= 0) 
            {
                trmList.RemoveAt(0);
                this.subTerms[nextIndex].connectTermsFromList(trmList);
            }
            // Otherwise the next term does not exist and we need to create it,
            // add it to this term's subTerms,
            // then traverse to it to add (potentially) more terms.
            else
            {
                Term newChild = new Term
                {
                    id = trmList[0].id,
                    rawTerm = trmList[0].rawTerm,
                    subTerms = new List<Term>(),
                };

                trmList.RemoveAt(0);
                this.subTerms.Add(newChild);
                this.subTerms[this.subTerms.Count - 1].connectTermsFromList(trmList);
            }
        }
    }
}
