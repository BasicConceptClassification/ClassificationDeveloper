using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Neo4j;
using BCCLib;

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
            Assert.AreEqual(results.data[0].name, "Adze Head");
            Assert.AreEqual(results.data[0].url, "www");
        }
    }
}
