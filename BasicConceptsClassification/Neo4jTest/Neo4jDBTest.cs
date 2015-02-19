using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Neo4j;
using BCCLib;
using System.Collections.Generic;
using System.Linq;

namespace Neo4jTest
{
    [TestClass]
    public class Neo4jDBTest
    {
        [TestMethod]
        public void TestConnection()
        {
             var neo4jdbConnection = new Neo4jDB();
             neo4jdbConnection.open();
             Assert.IsNotNull(neo4jdbConnection);
        }

        [TestMethod]
        public void TestCSQuery()
        {
            var TestConnection = new Neo4jDB();

            ClassifiableCollection results = 
                TestConnection.getClassifiablesByConStr(new BCCLib.ConceptString());

            // TODO: Fix these test Asserts
            // Should rely on test data once that's set up
            Assert.AreEqual(results.data[0].name, "Abrader");
            Assert.AreEqual(results.data[0].url, "https://sites.google.com/a/ualberta.ca/rick-szostak/publications/appendix-to-employing-a-synthetic-approach-to-subject-classification-across-glam/archaeology-object-name-list-used-by-the-us-national-parks-service");
            Assert.IsNull(results.data[0].tmpConceptStr, "");
        }

        [TestMethod]
        public void TestGetTermByRaw()
        {
            var TestConnection = new Neo4jDB();

            string rawT = "Art";
            Term term1 = TestConnection.getTermByRaw(rawT);

            Assert.AreEqual(rawT,term1.rawTerm);
            Assert.IsNotNull(term1.subTerms);
        }

        [TestMethod]
        public void TestGetChildrenOfTerm()
        {
            var TestConnection = new Neo4jDB();

            string rootRaw = "ROOT TERM";

            // These are the rawTerms that are one depth away from the root.
            List<string> rootSubTerms = new List<string>
            {
                "Art", 
                "Biological Entities", 
                "Celestial Objects",
                "Changes within a thing", 
                "Culture", 
                "Economy",
                "Flora and Fauna", 
                "Genetic Predisposition", 
                "Health and population", 
                "Individual differences",
                "Mathematical Concepts", 
                "Molecules and Elements",
                "Non-human Environment", 
                "Politics",
                "Properties, Qualities", 
                "Rocks", 
                "Social Structure",
                "Technology and Science", 
                "Waves and Particles",
            };

            Term rootTerm = TestConnection.getTermByRaw(rootRaw);

            rootTerm.subTerms = TestConnection.getChildrenOfTerm(rootTerm);

            // Make sure they're the same length
            Assert.AreEqual(rootSubTerms.Count, rootTerm.subTerms.Count);

            for (int i = 0; i < rootSubTerms.Count; i++)
            {
                Assert.AreEqual(rootSubTerms.ElementAt(i), rootTerm.subTerms.ElementAt(i).rawTerm);
                Assert.AreEqual(0, rootTerm.subTerms.ElementAt(1).subTerms.Count);
            }
        }

        [TestMethod]
        public void TestGetChildrenOfTermWithNoChildren()
        {
            var TestConnection = new Neo4jDB();
            string rawT = "Biology";

            Term Biology = TestConnection.getTermByRaw(rawT);
            Assert.IsNotNull(Biology);

            Biology.subTerms = TestConnection.getChildrenOfTerm(Biology);
            Assert.AreEqual(0,Biology.subTerms.Count);
        }

        [TestMethod]
        public void TestSearchForNonTerm()
        {
            var TestConnection = new Neo4jDB();

            Term term = new Term
            {
                rawTerm = "I am NOT a Term",
                subTerms = new List<Term>(),
            };

            Term resTerm = TestConnection.getTermByRaw(term.rawTerm);
            Assert.IsNull(resTerm);

            List<Term> resTerm2 = TestConnection.getChildrenOfTerm(term);
            Assert.AreEqual(0, resTerm2.Count);

            Term resTerm3 = TestConnection.getBccFromTermWithDepth(term, 0);
            Assert.IsNull(resTerm3);

            Term resTerm4 = TestConnection.getBccFromTermWithDepth(term, 2);
            Assert.IsNull(resTerm4);

            Term resTerm5 = TestConnection.getBccFromTerm(term);
            Assert.IsNull(resTerm5);
        }

        [TestMethod]
        public void TestGetBccFromTerm()
        {
            var TestConnection = new Neo4jDB();

            string rawT = "Natural Sciences";
            List<string> subT = new List<string>
            {
                "Biology",
                "Chemistry",
                "Computer science",
                "Geology",
                "Physics",
            };

            Term term = TestConnection.getTermByRaw(rawT);

            Term testRoot = TestConnection.getBccFromTermWithDepth(term, -1);

            Assert.AreEqual(rawT, testRoot.rawTerm);
            Assert.AreEqual(subT.Count, testRoot.subTerms.Count);

            // TODO: sort both lists alphabetiically, make asserts cleaner.
            // Expected results, not in a particular order.
            // Natural Sciences - Biology, 
            //                  - Chemistry
            //                  - Physics
            //                      - Atstronomy
            //                  - Computer Science
            //                  - Geology
            Assert.AreEqual(subT[0], testRoot.subTerms[0].rawTerm);
            Assert.AreEqual(0, testRoot.subTerms[0].subTerms.Count);
            Assert.AreEqual(subT[1], testRoot.subTerms[3].rawTerm);
            Assert.AreEqual(0, testRoot.subTerms[3].subTerms.Count);
            Assert.AreEqual(subT[2], testRoot.subTerms[2].rawTerm);
            Assert.AreEqual(0, testRoot.subTerms[2].subTerms.Count);
            Assert.AreEqual(subT[3], testRoot.subTerms[1].rawTerm);
            Assert.AreEqual(0, testRoot.subTerms[1].subTerms.Count);
            Assert.AreEqual(subT[4], testRoot.subTerms[4].rawTerm);
            Assert.AreEqual("astronomy", testRoot.subTerms[4].subTerms[0].rawTerm);
            Assert.AreEqual(0, testRoot.subTerms[4].subTerms[0].subTerms.Count);
        }

        [TestMethod]
        public void TestGetBccFromTermWIthDepth()
        {
            var TestConnection = new Neo4jDB();

            string rawT = "Waves and Particles";

            Term term = TestConnection.getTermByRaw(rawT);

            Term testRoot = TestConnection.getBccFromTermWithDepth(term, 2);

            // "Waves and Particles" has subTerms, and each of those subTerms
            // also has subTerms. Makes it a bit easier to test that we only get 
            // a depth of 2 for results when the subTerm results are (currently) 
            // not organized alphabetically. 
            Assert.AreEqual(rawT, testRoot.rawTerm);
            Assert.AreEqual(6, testRoot.subTerms.Count);
            Assert.AreNotEqual(0, testRoot.subTerms[0].subTerms.Count);
            Assert.AreEqual(0, testRoot.subTerms[0].subTerms[0].subTerms.Count);
        }


    }
}
