using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BCCLib;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace Neo4j
{
    class Neo4jDB
    {
        protected Uri dbLocation = new Uri("http://localhost:7474/db/data");
        protected GraphClient client;

        public void open()
        {
            client = new GraphClient(dbLocation);
            client.Connect();
        }


        public ClassifiableCollection query(ConceptString cstring)
        {
            return null;
        }
    }
}
