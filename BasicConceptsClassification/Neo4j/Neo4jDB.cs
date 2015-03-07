using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BCCLib;
using Neo4jClient;
using Neo4jClient.Cypher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        /// Gets a Classifiables by id
        /// </summary>
        /// <param name="id">The id of the Classifiable</param>
        /// <returns>A Classifiable with the given id.</returns>
        public Classifiable getClassifiableById(string id) 
        {
            this.open();
            if (client != null)
            {
                // Query:
                // MATCH (c:`Classifiable`{id:{searchId}})-[:HAS_CONSTR]->(cs) 
                // OPTIONAL MATCH (cs)-[:HAS_TERM]->(t:Term) 
                // RETURN 	c, 
		        //          cs,
		        //          COLLECT ([t]) as ts
                var query = client.Cypher
                    .Match("(c:Classifiable{id:{id}})-[:HAS_CONSTR]->(cs)")
                    .OptionalMatch("(cs)-[:HAS_TERM]->(t:Term)")
                    .WithParam("id", id)
                    .Return((c, t) => new
                    {
                        classifiable = c.As<Classifiable>(),
                        terms = t.CollectAs<Term>(),
                    }).Results.SingleOrDefault();

                if (query != null) {
                    // TODO: reorder terms to match concept string
                    ConceptString resConStr = new ConceptString
                    {
                        terms = new List<Term>(),
                    };

                    // Thanks muchy to the example:
                    // https://github.com/neo4j-contrib/developer-resources/blob/gh-pages/language-guides/dotnet/neo4jclient/Neo4jDotNetDemo/Controllers/MovieController.cs

                     foreach (var t in query.terms)
                     {
                            t.Data.subTerms = new List<Term>();
                            resConStr.terms.Add(t.Data);
                     }

                    // A bit of a hack for now. But it maintains order because of
                    // ...some reason.
                    resConStr.terms.Reverse();

                    query.classifiable.conceptStr = resConStr;

                    return query.classifiable;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a Classifiables by name. Since names are not unique,
        /// can get multiple results. If there are no results, the List will
        /// be empty.
        /// </summary>
        /// <param name="name">The name of the Classifiable</param>
        /// <returns>A ClassifiableCollection with its list of Classifiables
        /// if any Classifiables exist, otherwise the list will be empty.
        /// </returns>
        public ClassifiableCollection getClassifiablesByName(string name)
        {
            ClassifiableCollection resColl = new ClassifiableCollection
            {
                data = new List<Classifiable>(),
            };

            this.open();
            if (client != null)
            {
                // Query:
                // MATCH (c:`Classifiable`{id:{searchId}})-[:HAS_CONSTR]->(cs) 
                // OPTIONAL MATCH (cs)-[:HAS_TERM]->(t:Term) 
                // RETURN 	c, 
                //          cs,
                //          COLLECT ([t]) as ts
                var query = client.Cypher
                    .Match("(c:Classifiable{name:{name}})-[:HAS_CONSTR]->(cs)")
                    .OptionalMatch("(cs)-[:HAS_TERM]->(t:Term)")
                    .WithParam("name", name)
                    .Return((c, t) => new
                    {
                        classifiable = c.As<Classifiable>(),
                        terms = t.CollectAs<Term>(),
                    }).Results.ToList();

                if (query != null)
                {
                    foreach (var res in query)
                    {
                        // TODO: reorder terms to match concept string,
                        // other than the simple reversal later...
                        ConceptString resConStr = new ConceptString
                        {
                            terms = new List<Term>(),
                        };

                        // Getthe terms from the concept string
                        foreach (var t in res.terms)
                        {
                            t.Data.subTerms = new List<Term>();
                            resConStr.terms.Add(t.Data);
                        }

                        // A bit of a hack for now. But it maintains order because of
                        // ...some reason.
                        resConStr.terms.Reverse();

                        res.classifiable.conceptStr = resConStr;
                        
                        resColl.data.Add(res.classifiable);
                    }
                }
            }
            return resColl;
        }

        /// <summary>
        /// Get all the Classifiables that are not classified.
        /// <para>Classifiables returned are not associated with whoever
        /// added them.</para>
        /// </summary>
        /// <returns>A ClassifiableCollection with Classifiables that have
        /// not been classified. If all have been classified then the 
        /// the collection will be empty.</returns>
        public ClassifiableCollection getAllUnClassified()
        {
            ClassifiableCollection resColl = new ClassifiableCollection
            {
                data = new List<Classifiable>(),
            };

            this.open();
            if (client != null)
            {
                // Query 
                // MATCH (c:Classifiable)-[:`HAS_CONSTR`]->(cs:ConceptString {terms:""}) 
                // RETURN c, cs
                var query = client.Cypher
                    .Match("(c:Classifiable)-[:`HAS_CONSTR`]->(cs:ConceptString {terms:\"\"})")
                    .Return((c) => new
                    {
                        classifiable = c.As<Classifiable>(),
                    })
                    .Results.ToList();

                if (query != null)
                {
                    foreach (var res in query) 
                    {
                        res.classifiable.conceptStr = new ConceptString
                        {
                            terms = new List<Term>(),
                        };

                        resColl.data.Add(res.classifiable);
                    }
                }
            }
            return resColl;
        }

        /// <summary>
        /// Add a new Classifiable to the database.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Thrown when there is insufficient
        /// information for to add a Classifiable.</exception>
        /// <param name="newClassifiable">New Classifiable to add. Must have a Classifier.</param>
        /// <returns>The new Classifiable from the Database for verification.</returns>
        public Classifiable addClassifiable(Classifiable newClassifiable)
        {
            Classifiable rtnClassifiable = new Classifiable();

            this.open();
            if (client != null)
            {
                // Query: Merge classifier based on email, create new Classifiable and 
                // create relationship to the Classifier, split the ConStr into Terms and connect them.
                // MERGE (o:Classifier{email:{em}})
                // ON CREATE SET o.email ={em}
                // CREATE UNIQUE (c:Classifiable {id:{cId}})<-[:OWNS]-(o)
                // SET c.name="name-2",c.url = "url2", c.perm="perm2", c.status="status2"
                // CREATE UNQIUE (c)-[:HAS_CONSTR]->(cs:Classifiable)
                // SET cs.terms = "(new)(con)(str)"
                // WITH c, cs, REPLACE(cs.terms, "(", "") AS trmStr
                //      UNWIND ( FILTER
			    //          ( x in 
				//              SPLIT( trmStr, ")" ) 
				//              WHERE x <> ""
		  	    //          ) 
		        //      ) AS t4
                // MERGt:Term{lower:LOWER(t4)})
                // ON CREATE SET t.rawTerm=t4, t.lower=Lower(t4)
                // MERGE (cs)-[:HAS_TERM]->(t)
                // WITH c, cs, COLLECT([matchedT.rawTerm]) AS ts
                // RETURN c AS classifiable, ts AS terms
                // NOTE: Owner isn't returned from the actual DB at this point
                try
                {
                    var query = client.Cypher
                        .WithParams(new
                        {
                            cId = newClassifiable.owner.getOrganizationName() + 
                                "_" + 
                                newClassifiable.name,
                            cName = newClassifiable.name,
                            cUrl = newClassifiable.url,
                            cPerm = newClassifiable.perm,
                            cStatus = newClassifiable.status,
                            em = newClassifiable.owner.email,
                            newConStr = newClassifiable.conceptStr.ToString()
                        })
                        .Merge("(o:Classifier{email:{em}})")
                        .OnCreate()
                        .Set("o.email ={em}")
                        .CreateUnique("(c:Classifiable {id:{cId}})<-[:OWNS]-(o)")
                        .Set("c.id = {cId}, c.name = {cName}, c.url = {cUrl}, c.perm = {cPerm}, c.status = {cStatus}")
                        .CreateUnique("(c)-[:HAS_CONSTR]->(cs:ConceptString)")
                        .Set("cs.terms = {newConStr}")
                        .With(@"c, cs, REPLACE({newConStr}, ""("", """") AS trmStr
                                    UNWIND ( FILTER
                                                ( x in
                                                    SPLIT( trmStr, "")"" )
                                                    WHERE x <> """"
                                                )
                                            ) AS t4")
                        .Match("(matchedT:Term {rawTerm: t4})")
                        .Merge("(cs)-[:HAS_TERM]->(matchedT)")
                        .With("c, COLLECT([matchedT.rawTerm]) AS ts")
                        .Return((c, ts) => new
                        {
                            classifiable = c.As<Classifiable>(),
                            terms = ts.As<IEnumerable<string>>(),
                            //owner = Return.As<IEnumerable<string>>("COLLECT[o.email])"),
                        })
                        .Results.ToList().Single();

                    if (query != null)
                    {
                        // Construct the Concept String from results
                        ConceptString resConStr = new ConceptString
                        {
                            terms = new List<Term>(),
                        };

                        // Build the terms
                        foreach (var t in query.terms)
                        {
                            var tempData = JsonConvert.DeserializeObject<dynamic>(t);
                            var tmp = new Term
                            {
                                rawTerm = tempData[0]
                            };

                            tmp.subTerms = new List<Term>();
                            resConStr.terms.Add(tmp);
                        }
                   
                        rtnClassifiable = query.classifiable;
                        rtnClassifiable.owner = newClassifiable.owner;

                        // Reverse for some reason
                        resConStr.terms.Reverse();
                        rtnClassifiable.conceptStr = resConStr;

                        return rtnClassifiable;
                    }
                }
                catch (NullReferenceException e) 
                {
                    Console.WriteLine("Classifiable information missing or Classifier email was not set", e);
                    throw new NullReferenceException(@"Classifiable information missing or Classifier email was not set", e);
                }
            }
            return null;
        }

        /// <summary>
        /// Deletes a classifiable
        /// </summary>
        /// <param name="classifiable"></param>
        public void deleteClassifiable(Classifiable classifiable)
        {   
            this.open();
            if (client != null) {
                // MATCH (:Classifier)-[r]-(a{id:"Neo4j-dummyiD"})-[r2:`HAS_CONSTR`]->(b)
                // OPTIONAL MATCH (b)-[r3:HAS_TERM]->(t) 
                // DELETE r, a, r2, b, r3
                client.Cypher
                    .Match("(:Classifier)-[r]-(c:Classifiable {id:{cId}})")
                    .OptionalMatch("(c)-[r1:HAS_CONSTR]->(cs:ConceptString)-[r2:HAS_TERM]->(:Term)")
                    .WithParam("cId", classifiable.id)
                    .Delete("r, c, r1, cs, r2")
                    .ExecuteWithoutResults();
            }
        }

        /// <summary>
        /// Not finished. Will do as name implies.
        /// </summary>
        /// <param name="updatedClassifiable"></param>
        /// <returns></returns>
        public Classifiable updateClassifiable(Classifiable updatedClassifiable)
        {
            //.OnMatch()
            // .Set("c.id = {cId}, c.name = {cName}, c.url = {cUrl}, c.perm = {cPerm}, c.status = {cStatus}")
            return null;
        }

        /// <summary>
        /// Queries the database to return Classifiables based on a ConceptString.
        /// Use the limit and skip parameters to page the results. 
        /// </summary>
        /// <param name="cstring">A Concepttring to search by.</param>
        /// <returns>Returns a ClassifiableCollection where each Classifiable's 
        /// ConceptSring has at least one matching term from </returns>
        public ClassifiableCollection getClassifiablesByConStr(ConceptString conStr, bool ordered = false)
        {
            ClassifiableCollection resColl = new ClassifiableCollection
            {
                data = new List<Classifiable>(),
            };
                
            this.open();

            if (client != null)
            {

                // Building one of the following:
                // WHERE ( { t.id= {id_0}) OR .. OR (t.id = id_n } )
                // WHERE ( { t.rawTerm = {rawTerm_0}) OR .. OR (t.rawTerm = rawTerm_n } )
                // if going by rawTerms, maybe try to match by lower?
                string whereClause = "";

                foreach (Term t in conStr.terms)
                {
                    whereClause += String.Format(" (t.rawTerm = \"{0}\") OR", t.rawTerm);
                }

                whereClause += " (t.rawTerm = \"\")";

                // Query idea - first part will collect, but not ordered
                // by the number of matches. Second query does. Not sure what needs
                // to be carried over/returned, but anyways...
                // Ref: http://architects.dzone.com/articles/neo4jcypher-combining-count
                // Query:

                // MATCH (c:Classifiable)-[:HAS_CONSTR]->(cs:ConceptString)-[:HAS_TERM]->(t:Term)
                // WHERE ( { t.rawTerm = {rawTerm_0}) OR .. OR (t.rawTerm = rawTerm_n } )
                // WITH DISTINCT    c, 
                //                  cs, 
                //                  COUNT([t]) AS numMatched

                // MATCH (c:Classifiable)-[:HAS_CONSTR]->(cs:ConceptString)-[:HAS_TERM]->(t2:Term)
                // WITH DISTINCT    c, 
                //                  cs, 
                //                  COLLECT([t2]) AS terms,
                //                  numMatched
             
                // RETURN c AS classifiable, terms
                // ORDER BY numMatched DESC
                // SKIP {skip} LIMIT {limit}
                var query = client.Cypher
                    .Match("(c:Classifiable)-[:HAS_CONSTR]->(cs:ConceptString)-[:HAS_TERM]->(t:Term)")
                    .Where(whereClause)
                    .With("DISTINCT c, cs, COUNT([t]) AS numMatched")
                    .Match("(c)-[:HAS_CONSTR]->(cs)-[:HAS_TERM]->(t)")
                    .With("DISTINCT c, COLLECT([t.id, t.rawTerm]) as terms, numMatched");

                if (ordered) 
                {
                    query = query.OrderBy("numMatched DESC");
                }
                  
                var results = query.Return((c, terms) => new
                    {
                        classifiable = c.As<Classifiable>(),
                        terms = terms.As<IEnumerable<string>>(),
                    })
                    .Results.ToList();


                if (query != null)
                {
                    // Build up Classifiables
                    for (int i = 0; i < results.Count; i++)
                    {
                        var res = results.ElementAt(i);

                        // TODO: reorder terms to match concept string,
                        // other than the simple reversal later...
                        ConceptString resConStr = new ConceptString
                        {
                            terms = new List<Term>(),
                        };
                     
                        // Get the terms from the concept string
                        foreach (var t in res.terms)
                        {
                            var tempData = JsonConvert.DeserializeObject<dynamic>(t);
                            var tmp = new Term
                            {
                                id = tempData[0],
                                rawTerm = tempData[1],
                            };

                            tmp.subTerms = new List<Term>();
                            resConStr.terms.Add(tmp);
                        }
                       
                        // A bit of a hack for now. But it maintains order because of
                        // ...some reason.
                        resConStr.terms.Reverse();
                        
                        Classifiable cTmp = new Classifiable
                        {
                            name = res.classifiable.name,
                            id = res.classifiable.id,
                            url = res.classifiable.url,
                            conceptStr = resConStr,
                        };

                        resColl.data.Add(cTmp);
                    }
                }
            }
            return resColl;
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