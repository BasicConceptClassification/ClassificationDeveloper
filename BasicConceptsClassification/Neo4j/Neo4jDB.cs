using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BCCLib;
using Neo4jClient;
using Neo4jClient.Cypher;

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

            var finalResult = new ClassifiableCollection
            {
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
        /// <para>Do not use to get the root term pf the BCC. 
        /// Use getRootTerm() instead.</para>
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
                    .Return((t) => new
                    {
                        trm = t.As<Term>(),
                    })
                    .Results.ToList();

                if (query.Count != 0)
                {
                    query[0].trm.subTerms = getChildrenOfTerm(query[0].trm);
                    return query[0].trm;
                }
            }
            return null;
        }

        /// <summary>
        /// Queries the database for a Term based on lower, the lower
        /// case version of a Term's raw term.
        /// <para>Do not use to get the root term pf the BCC. 
        /// Use getRootTerm() instead.</para>
        /// </summary>
        /// <param name="rTerm">The lower case form of a raw term 
        /// to search by.</param>
        /// <returns>Returns a Term with its rawTerm and empty subTerm list 
        /// if it exists, null otherwise.</returns>
        public Term getTermByLower(string lower)
        {
            this.open();

            if (client != null)
            {
                // Query:
                // MATCH (t:Term {rawTerm:{raw}})
                // RETURN t
                var query = client.Cypher
                    .Match("( t:Term {lower:{lower}} )")
                    .WithParam("lower", lower)
                    .Return((t) => new
                    {
                        trm = t.As<Term>(),
                    })
                    .Results.ToList();

                if (query.Count != 0)
                {
                    query[0].trm.subTerms = getChildrenOfTerm(query[0].trm);
                    return query[0].trm;
                }
            }
            return null;
        }

        /// <summary>
        /// Queries the database for the sub terms for the provided Term.
        /// Each sub term found will have an empty subTerm List of Terms.
        /// </summary>
        /// <param name="term">The Term to find the children of.</param>
        /// <returns>A List of the children in alphabetical order by rawTerm 
        /// if they exist. Otherwise, returns an empty List of Terms.</returns>
        public List<Term> getChildrenOfTerm(Term term)
        {
            this.open();

            List<Term> children = new List<Term>();

            if (client != null)
            {
                // Query:
                // MATCH ({rawTerm:{rawT}})<-[:SUBTERM_OF]-(a:Term) 
                // WITH a 
                // ORDER BY a.rawTerm 
                // RETURN a
                var query = client.Cypher
                    .Match("({rawTerm:{rawT}})<-[:SUBTERM_OF]-(a:Term) ")
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
                        q.sub.subTerms = new List<Term>();
                        children.Add(q.sub);
                    }
                }
            }
            return children;
        }

        /// <summary>
        /// Gets the entire tree from the specified specified Term downwards.
        /// </summary>
        /// <param name="rootTerm">The term that will act as the root.</param>
        /// <returns>A new(?) root term term with all of its children
        /// as deep as it can go.</returns>
        public Term getBccFromTerm(Term rootTerm)
        {
            this.open();

            if (client != null)
            {
                // Query:
                // MATCH p=(pT {rawTerm:{rawT}})<-[:SUBTERM_OF*]-(cT:Term)"
                // WHERE not ( cT<-[:SUBTERM_OF]-() )
                // RETURN nodes(p)
                // Thanks to: http://wes.skeweredrook.com/cypher-longest-path/
                // TODO: would like to have at each depth for everything to
                // be sorted alphabetically.
                // And as long as there isn't any CYCLES, this should be fine...
                var query = client.Cypher
                    .Match("p=(pT {rawTerm:{rawT}})<-[:SUBTERM_OF*]-(cT:Term)")
                    .WithParam("rawT", rootTerm.rawTerm)
                    .Where("not ( cT<-[:SUBTERM_OF]-() )")
                    .Return(() => Return.As<List<Term>>("nodes(p)"))
                    .Results.ToList();

                if (query.Count != 0)
                {
                    // Recreate the root term
                    Term resTree = new Term
                    {
                        id = query.ElementAt(0).ElementAt(0).id,
                        rawTerm = query.ElementAt(0).ElementAt(0).rawTerm,
                        lower = query.ElementAt(0).ElementAt(0).lower,
                        subTerms = new List<Term>()
                    };

                    // Connect the subterms from a list. Remove first since
                    // it's the root term.
                    foreach (List<Term> path in query)
                    {
                        path.RemoveAt(0);
                        resTree.connectTermsFromList(path);
                    }
                    return resTree;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the Classification from the requested term down to the requested
        /// depth. If the intention is to get the entire tree, it is MUCH better
        /// to use the other one, which this will call by default if a bizarre
        /// depth is given.
        /// <para>A depth of 0 will just return the Term itself.</para>
        /// <param name="rootTerm">The term that will act as the root.</param>
        /// <param name="depth">How much depth the sub terms of the root term
        /// will have.</param>
        /// <returns>A new(?) Term as the root with all of its sub terms 
        /// going as deep as the specified depth.</returns>
        public Term getBccFromTermWithDepth(Term rootTerm, int depth)
        {
            this.open();

            if (client != null)
            {
                // Don't really want to build it this way, but for now?
                string matchStr = "p=(pT {rawTerm: {rawT} })<-[:SUBTERM_OF*";
                if (depth >= 0)
                {
                    matchStr += "0.." + depth.ToString();
                }
                // If this method is called with some non-sensical integer, 
                // it's just going to call getBccFromTErm and you're getting 
                // all the sub terms.
                else 
                {
                    return this.getBccFromTerm(rootTerm);
                }
                matchStr += "]-(cT:Term)";

                // Query I would like::
                // MATCH p=(pT {rawTerm:{rawT}})<-[:SUBTERM_OF*{depth}]-(cT:Term)"
                // WHERE not ( cT<-[:SUBTERM_OF]-() )
                // RETURN nodes(p)
                // But only using WithParams() and not string formatting.
                // "[:SUBTERM*{depth}]" to be "[:SUBTERM_OF*0..{depth}]"
                // Unfortunately this gives repeated paths: (a)->(b), (a)->(b)->(c).
                // Haven't figured out how to get a similar result to the 
                // getBccFromTerm() method below which only gets the longest path.
                var queryRes = client.Cypher
                    .Match(matchStr)
                    .WithParam("rawT", rootTerm.rawTerm)
                    .Return(() => Return.As<List<Term>>("nodes(p)"))
                    .Results.ToList();

                if (queryRes.Count != 0)
                {
                    // Recreate the root term
                    Term resTree = new Term
                    {
                        id = queryRes.ElementAt(0).ElementAt(0).id,
                        rawTerm = queryRes.ElementAt(0).ElementAt(0).rawTerm,
                        lower = queryRes.ElementAt(0).ElementAt(0).lower,
                        subTerms = new List<Term>()
                    };

                    // Connect the subterms from a list. Remove first since
                    // it's the root term.
                    foreach (List<Term> path in queryRes) 
                    {
                        path.RemoveAt(0);
                        resTree.connectTermsFromList(path);
                    }
                    return resTree;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the root term of the BCC.
        /// </summary>
        /// <returns>The Term that is the root of the BCC.</returns>
        public Term getRootTerm()
        {
            this.open();

            if (client != null)
            {
                // Query:
                // MATCH (top:BccRoot)
                // RETURN top
                var query = client.Cypher
                    .Match("(top:BccRoot)")
                    .Return(() => Return.As<Term>("top"))
                    .Results.ToList();

                if (query != null)
                {
                    query.ElementAt(0).subTerms = new List<Term>();
                    return query.ElementAt(0);
                }
            }
            return null;
        }

        /// <summary>
        /// Get the BCC from the root to the specified depth.
        /// </summary>
        /// <param name="depth">Depth of the subTerms of the root.</param>
        /// <returns>The BCC Root Term with the specified subTerm 
        /// depth. Returns null if there was issues.</returns>
        public Term getBccFromRootWithDepth(int depth)
        {
            Term tmpRoot = getRootTerm();

            if (tmpRoot != null)
            {
                return getBccFromTermWithDepth(tmpRoot, depth);
            }
            return tmpRoot;
        }
    }
}