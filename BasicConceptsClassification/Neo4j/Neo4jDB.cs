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
                .Match("(c:Classifiable)-[CLASSIFIED_TERMS]->(cs:ConceptString)")
                .Return((c, cs) => new
                {
                    c = c.As<Classifiable>(),
                    conceptString = Return.As<string>("cs.name"),
                })
                .Results;

            var finalResult = new ClassifiableCollection { 
                data = new List<Classifiable>(),
            };
            
            foreach (var results in query)
            {
                
                Classifiable dummy = new Classifiable
                {
                    name = query.ElementAt(0).c.name,
                    url = query.ElementAt(0).c.url,
                    tmpConceptStr = query.ElementAt(0).conceptString,
                };

                finalResult.data.Add(dummy);
            }

            return finalResult;
        }
    }
}
