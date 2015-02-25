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
        public void HasSubTerm_TermWithSubTerms_Found()
        {
            Term term = new Term
            {
                id = "id01",
                rawTerm = "Raw",
                subTerms = new List<Term>(),
            };

            Term subTerm = new Term
            {
                id = "id02",
                rawTerm = "subRaw",
                subTerms = new List<Term>(),
            };

            term.subTerms.Add(subTerm);

            int foundSubTermIndex = term.hasSubTerm(subTerm);

            Assert.AreEqual(0, foundSubTermIndex);
            Assert.AreEqual(subTerm, term.subTerms[foundSubTermIndex]);
        }

        [TestMethod]
        public void HasSubTerm_TermWithSubTerms_NotFound()
        {
            int NOT_FOUND = -1;

            Term term = new Term
            {
                id = "id01",
                rawTerm = "Raw",
                subTerms = new List<Term>(),
            };

            Term subTerm1 = new Term
            {
                id = "id02",
                rawTerm = "subRaw",
                subTerms = new List<Term>(),
            };

            Term subTerm2 = new Term
            {
                id = "id03",
                rawTerm = "Raw2",
                subTerms = new List<Term>(),
            };

            term.subTerms.Add(subTerm1);

            int subTermNotFoundIndex = term.hasSubTerm(subTerm2);

            Assert.AreEqual(NOT_FOUND, subTermNotFoundIndex);
        }

        [TestMethod]
        public void HasSubTerm_TermWithNoSubTerms_NotFound()
        {
            int NOT_FOUND = -1;

            Term term = new Term
            {
                id = "id01",
                rawTerm = "Raw",
                subTerms = new List<Term>(),
            };

            Term subTerm = new Term
            {
                id = "id02",
                rawTerm = "subRaw",
                subTerms = new List<Term>(),
            };

            int notFoundSubTermIndex = term.hasSubTerm(subTerm);

            Assert.AreEqual(NOT_FOUND, notFoundSubTermIndex);
        }

        [TestMethod]
        public void ConnectTermsFromList_SingleTermListExists() 
        {
            Term term = new Term
            {
                id = "id01",
                rawTerm = "Raw",
                subTerms = new List<Term>(),
            };

            Term subTerm1 = new Term
            {
                id = "id02",
                rawTerm = "subRaw",
                subTerms = new List<Term>(),
            };

            Term subTerm2 = new Term
            {
                id = "id03",
                rawTerm = "subRaw3",
                subTerms = new List<Term>(),
            };

            List<Term> subTermList = new List<Term> 
            {
                subTerm1, subTerm2,
            };

            term.connectTermsFromList(subTermList);

            // Expect the following structure:
            // t.subTerms has subT, subT.subTerms has subT2 
            Assert.AreEqual(1, term.subTerms.Count);
            Assert.AreEqual(subTerm1.rawTerm, term.subTerms[0].rawTerm);
            Assert.AreEqual(1, term.subTerms[0].subTerms.Count);
            Assert.AreEqual(subTerm2.rawTerm, term.subTerms[0].subTerms[0].rawTerm);
            Assert.AreEqual(0, term.subTerms[0].subTerms[0].subTerms.Count);
        }
    
        [TestMethod]
        public void Term_ToString_HasRawTerm()
        {
            Term t = new Term
            {
                id = "id01",
                rawTerm = "Raw",
                lower = "raw",
                subTerms = new List<Term>(),
            };

            string raw = t.ToString();
            Assert.AreEqual("(Raw)", raw);
        }
    }
}
