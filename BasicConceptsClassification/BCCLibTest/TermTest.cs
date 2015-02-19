using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BCCLib;
using System.Collections.Generic;

namespace BCCLibTest
{
    [TestClass]
    public class TermTest
    {
        [TestMethod]
        public void TestHasSubTerm_Found()
        {
            Term t = new Term
            {
                id = "id01",
                rawTerm = "Raw",
                subTerms = new List<Term>(),
            };

            Term subT = new Term
            {
                id = "id02",
                rawTerm = "subRaw",
                subTerms = new List<Term>(),
            };

            t.subTerms.Add(subT);

            int index = t.hasSubTerm(subT);

            Assert.AreEqual(0, index);
            Assert.AreEqual(subT, t.subTerms[index]);
        }

        [TestMethod]
        public void TestHasSubTerm_NotFound()
        {
            Term t = new Term
            {
                id = "id01",
                rawTerm = "Raw",
                subTerms = new List<Term>(),
            };

            Term subT = new Term
            {
                id = "id02",
                rawTerm = "subRaw",
                subTerms = new List<Term>(),
            };

            Term t2 = new Term
            {
                id = "id03",
                rawTerm = "Raw2",
                subTerms = new List<Term>(),
            };

            t.subTerms.Add(subT);

            int index = t.hasSubTerm(t2);

            Assert.AreEqual(-1, index);
        }

        [TestMethod]
        public void TestHasSubTerm_NoSubTerms()
        {
            Term t = new Term
            {
                id = "id01",
                rawTerm = "Raw",
                subTerms = new List<Term>(),
            };

            Term subT = new Term
            {
                id = "id02",
                rawTerm = "subRaw",
                subTerms = new List<Term>(),
            };

            int index = t.hasSubTerm(subT);

            Assert.AreEqual(-1, index);
        }

        [TestMethod]
        public void TestConnectTermsFromList() 
        {
            Term t = new Term
            {
                id = "id01",
                rawTerm = "Raw",
                subTerms = new List<Term>(),
            };

            Term subT = new Term
            {
                id = "id02",
                rawTerm = "subRaw",
                subTerms = new List<Term>(),
            };

            Term subT2 = new Term
            {
                id = "id03",
                rawTerm = "subRaw3",
                subTerms = new List<Term>(),
            };

            List<Term> termList = new List<Term> 
            {
                subT, subT2,
            };

            t.connectTermsFromList(termList);

            // Expect the following structure:
            // t.subTerms has subT, subT.subTerms has subT2 
            Assert.AreEqual(1, t.subTerms.Count);
            Assert.AreEqual(subT.rawTerm, t.subTerms[0].rawTerm);
            Assert.AreEqual(1, t.subTerms[0].subTerms.Count);
            Assert.AreEqual(subT2.rawTerm, t.subTerms[0].subTerms[0].rawTerm);
            Assert.AreEqual(0, t.subTerms[0].subTerms[0].subTerms.Count);
        }
    
        [TestMethod]
        public void TestToString()
        {
            Term t = new Term
            {
                id = "id01",
                rawTerm = "Raw",
                subTerms = new List<Term>(),
            };

            string raw = t.ToString();
            Assert.AreEqual("(Raw)", raw);
        }
    }
}
