using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Neo4j;
using BCCLib;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;

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
        public void TestGetClassifiableById()
        {
            var TestConn = new Neo4jDB();

            int id = 2;
            Classifiable classy = TestConn.getClassifiableById(id);

            Assert.IsNotNull(classy);
            Assert.AreEqual(id.ToString(), classy.id);
            Assert.AreEqual("Adze Blade", classy.name);
            Assert.AreEqual(6, classy.conceptStr.terms.Count);
            Assert.AreEqual("(blade)(of)(Tool)(for)(carving)(wood)", classy.conceptStr.ToString());
        }

        [TestMethod]
        public void TestGetClassifiableById_NotClassified()
        {
            var TestConn = new Neo4jDB();

            int id = 14;
            Classifiable classy = TestConn.getClassifiableById(id);

            Assert.IsNotNull(classy);
            Assert.AreEqual(id.ToString(), classy.id);
            Assert.AreEqual(0, classy.conceptStr.terms.Count);
            Assert.AreEqual("", classy.conceptStr.ToString());
        }

        [TestMethod]
        public void TestGetClassifiableById_NotInDb()
        {
            var TestConn = new Neo4jDB();

            int id = 909090;
            Classifiable classy = TestConn.getClassifiableById(id);

            Assert.IsNull(classy);
        }

        [TestMethod]
        public void TestGetClassifiableByName_NotInDb()
        {
            var TestConn = new Neo4jDB();

            string name = "909090";
            ClassifiableCollection classyColl = TestConn.getClassifiablesByName(name);

            Assert.IsNotNull(classyColl);
            Assert.AreEqual(0, classyColl.data.Count);
        }

        [TestMethod]
        public void TestGetClassiableByName() 
        {
            var TestConn = new Neo4jDB();

            string searchName = "Adze Blade";
            ClassifiableCollection classyColl = TestConn.getClassifiablesByName(searchName);

            Assert.IsNotNull(classyColl.data);
            Assert.AreEqual(searchName, classyColl.data[0].name);
            Assert.AreEqual(6, classyColl.data[0].conceptStr.terms.Count);
            Assert.AreEqual("(blade)(of)(Tool)(for)(carving)(wood)", classyColl.data[0].conceptStr.ToString());
        }

        [TestMethod]
        public void TestGetAllUnClassified()
        {
            var TestConn = new Neo4jDB();

            ClassifiableCollection classyColl = TestConn.getAllUnClassified();

            foreach (var classy in classyColl.data)
            {
                Assert.AreEqual(0, classy.conceptStr.terms.Count);
                Assert.AreEqual("", classy.conceptStr.ToString());
            }
        }

        [TestMethod]
        public void TestCSQuery()
        {
            var TestConnection = new Neo4jDB();

            ClassifiableCollection results = 
                TestConnection.getClassifiablesByConStr(new ConceptString());

            // TODO: Fix these test Asserts
            // Should rely on test data once that's set up
            Assert.IsNotNull(results);
            Assert.AreEqual(results.data[0].name, "Abrader");
            Assert.AreEqual("(tool)(for)(smoothing)", results.data[0].tmpConceptStr);
        }

        [TestMethod]
        public void TestGetTermByRaw()
        {
            var TestConnection = new Neo4jDB();

            string rawT = "Art";
            Term term1 = TestConnection.getTermByRaw(rawT);

            Assert.AreEqual(rawT,term1.rawTerm);
            Assert.AreEqual("art", term1.lower);
            Assert.IsNotNull(term1.id);
            Assert.IsNotNull(term1.subTerms);
        }

        [TestMethod]
        public void TestGetTermByLower()
        {
            var TestConnection = new Neo4jDB();

            string lower = "art";
            Term term1 = TestConnection.getTermByLower(lower);

            Assert.AreEqual(lower, term1.lower);
            Assert.AreEqual("Art", term1.rawTerm);
            Assert.IsNotNull(term1.id);
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
        public void TestGetBccFromTermWithDepth()
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
    
        [TestMethod]
        public void TestGetRootTerm()
        {
            var TestConnection = new Neo4jDB();

            Term rootTerm = TestConnection.getRootTerm();

            Assert.IsNotNull(rootTerm);
            Assert.AreEqual("bccRoot", rootTerm.id);
            Assert.AreEqual("bccroot", rootTerm.lower);
            Assert.AreEqual("BccRoot", rootTerm.rawTerm);
            Assert.IsNotNull(rootTerm.subTerms);
            Assert.AreEqual(0, rootTerm.subTerms.Count);
        }

        [TestMethod]
        public void TestGetBccFromRootWithDepth()
        {
            Term term1 = new Term
            {
                rawTerm = "ROOT TERM",
            };

            Term term2 = new Term
            {
                rawTerm = "TOP OBJECT PROPERTY",
            };

            Term term3 = new Term
            {
                rawTerm = "Not a Real Term",
            };

            var TestConnection = new Neo4jDB();

            Term bccRoot = TestConnection.getBccFromRootWithDepth(1);

            Assert.IsNotNull(bccRoot);
            Assert.AreEqual("bccroot", bccRoot.lower);
            Assert.IsNotNull(bccRoot.subTerms.Count);
            Assert.IsNotNull(bccRoot.hasSubTerm(term1));
            Assert.IsNotNull(bccRoot.hasSubTerm(term2));
            Assert.IsNotNull(bccRoot.hasSubTerm(term3));
        }
    }
}