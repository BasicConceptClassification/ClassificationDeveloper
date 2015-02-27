using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BCCLib;
using System.Collections.Generic;

namespace BCCLibTest
{
    [TestClass]
    public class ConceptStringTest
    {
        [TestMethod]
        public void ConceptString_ToString_HasTerms()
        {
            Term t1 = new Term 
            { 
                id = "tmpId01",
                rawTerm = "Term 1",
                lower = "term 1",
                subTerms = new List<Term>(),
            };

            Term t2 = new Term 
            { 
                id = "tmpId02",
                rawTerm = "Term 2",
                lower = "term 2",
                subTerms = new List<Term>(),
            };

            ConceptString testConStr = new ConceptString 
            {
                terms = new List<Term> {
                    t1, t2,
                },
            };

            Assert.AreEqual("(Term 1)(Term 2)", testConStr.ToString());
        }
    }
}
