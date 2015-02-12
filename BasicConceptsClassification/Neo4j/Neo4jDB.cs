using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BCCLib;
using Neo4jClient;
using Neo4jClient.Cypher;

using System.Diagnostics;

namespace Neo4j
{
    public class Neo4jDB
    {
        protected Uri dbLocation = new Uri("http://localhost:7474/db/data");
        protected GraphClient client;

        public void open()
        {
            client = new GraphClient(dbLocation);
            client.Connect();
        }

        public ClassifiableCollection getClassifiablesByConceptString(ConceptString cstring)
        {
            this.open();

            var query = client.Cypher
                .Match("(c:Classifiable)")
                .Return(c => c.As<Classifiable>())
                .Results;

            return new ClassifiableCollection
            {
                data = query.Cast<Classifiable>().ToList(),
            };
        }
    }
}
