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

        /// <summary>
        /// Open the connection to the database.
        /// </summary>
        public void open()
        {
            client = new GraphClient(dbLocation);
            client.Connect();
        }

        /// <summary>
        /// Queries the database to return Classifiables based on a ConceptString.
        /// </summary>
        /// <param name="cstring">A ConceptString to search by.</param>
        /// <returns>Returns a ClassifiableCollection</returns>
        public ClassifiableCollection getClassifiablesByConStr(ConceptString cstring)
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

        /// <summary>
        /// Queries the database for a Term based on a raw term.
        /// </summary>
        /// <param name="rTerm">The raw term to search by.</param>
        /// <returns>Returns a Term with its rawTerm and empty subTerm list 
        /// if it exists, null otherwise.</returns>
        public Term getTermByRaw(string rTerm)
        {
            this.open();

            if (client != null)
            {
                // Query:
                // MATCH (t:Term {rawTerm:{raw}})
                // RETURN t
                var query = client.Cypher
                    .Match("( t:Term {rawTerm:{raw}} )")
                    .WithParam("raw", rTerm)
                    .Return((t) => new {
                        trm = t.As<Term>(),
                    })
                    .Results.ToList();

                if (query.Count != 0)
                {
                    return query[0].trm;
                }
            }
            return null;
        }

        /// <summary>
        /// Queries the database for the children of the provided Term.
        /// </summary>
        /// <param name="term">The Term to find the children of</param>
        /// <returns>A List of the children in alphabetical order by rawTerm 
        /// if they exist. Otherwise, returns an empty List of Terms.</returns>
        public List<Term> getChildrenOfTerm(Term term)
        {
            this.open();

            List<Term> children = new List<Term>();

            if (client != null)
            {
                // Query:
                // MATCH (:Term{rawTerm:{rawT}})<-[:SUBTERM_OF]-(a:Term) 
                // WITH a 
                // ORDER BY a.rawTerm 
                // RETURN a
                var query = client.Cypher
                    .Match("(:Term{rawTerm:{rawT}})<-[:SUBTERM_OF]-(a:Term) ")
                    .WithParam("rawT", term.rawTerm)
                    .With("a")
                    .OrderBy("a.rawTerm")
                    .Return((a) => new
                     {
                         sub = Return.As<Term>("a"),
                     })
                    .Results.ToList();

                if (query.Count != 0)
                {
                    foreach (var q in query) 
                    {
                        children.Add(q.sub);
                    }
                }
            }
            return children;
        }
    }
}
