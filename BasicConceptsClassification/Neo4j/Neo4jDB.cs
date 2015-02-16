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
                .Match("(c:Classifiable)-[HAS_CONSTR]->(cs:ConceptString)")
                .Return((c, cs) => new
                {
                    c = c.As<Classifiable>(),
                    conceptString = Return.As<string>("cs.name"),
                })
                .Results;

            var finalResult = new ClassifiableCollection { 
                data = new List<Classifiable>(),
            };
      
            // Build up Classifiables
            foreach (var result in query)
            {    
                Classifiable dummy = new Classifiable
                {
                    name = result.c.name,
                    url = result.c.url,
                    tmpConceptStr = result.conceptString,
                };

                finalResult.data.Add(dummy);
            }

            return finalResult;
        }

        public Term getOneBccDepthAtTerm(Term term)
        {
            this.open();
            
            // Query:
            // MATCH (b)-->(a {name:"ROOT TERM"}) WITH a, b
            // ORDER BY b.name
            // RETURN a, b
            var query = client.Cypher
                .Match("(b:Term)-->(a:Term { rawTerm:{termStr} })")
                .WithParam("termStr", term.rawTerm)
                .With("a,b")
                .OrderBy("b.rawTerm")
                .Return((a, b) => new
                {
                    raw = Return.As<string>("a.rawTerm"),
                    sub = Return.As<string>("b.rawTerm"),
                })
                .Results;

            // set up raw term
            var finalResult = new Term
            {
                rawTerm = query.ElementAt(0).raw,
                subTerms = new List<Term>(),
            };

            // Construct sub terms
            foreach (var result in query) {
                Term dummyTerm = new Term
                {
                    rawTerm = result.sub,
                    subTerms = new List<Term>(),
                };

                finalResult.subTerms.Add( dummyTerm );
            }
            return finalResult;
        }
    }
}
