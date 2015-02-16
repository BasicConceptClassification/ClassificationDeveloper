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
            var Neo4jTestConnection = new Neo4jDB();

            ClassifiableCollection results = 
                Neo4jTestConnection.getClassifiablesByConceptString(new BCCLib.ConceptString());

            // TODO: Fix these test Asserts
            // Should rely on test data once that's set up
            Assert.AreEqual(results.data[0].name, "Abrader");
            Assert.AreEqual(results.data[0].url, "https://sites.google.com/a/ualberta.ca/rick-szostak/publications/appendix-to-employing-a-synthetic-approach-to-subject-classification-across-glam/archaeology-object-name-list-used-by-the-us-national-parks-service");
            Assert.IsNull(results.data[0].tmpConceptStr, "");
        }

        [TestMethod]
        public void getRootBccOneDepth()
        {
            var TestConnection = new Neo4jDB();

            // We want to start at the root term, so here it is
            Term rootTerm = new Term{
                rawTerm = "ROOT TERM",
                subTerms = new List<Term>(),
            };

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

            Term BccRootOneDepth = TestConnection.getOneBccDepthAtTerm(rootTerm);

            // Make sure they're the same length
            Assert.AreEqual(rootSubTerms.Count, BccRootOneDepth.subTerms.Count);
            
            for (int i = 0; i < rootSubTerms.Count; i++ )
            {
                Assert.AreEqual(rootSubTerms.ElementAt(i), 
                    BccRootOneDepth.subTerms.ElementAt(i).rawTerm);
            }
        }
    }
}
