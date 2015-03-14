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

        /* TODO:
         * We should define all of the DB constants as class constants to make
         * refactoring the db less of a PITA. Then again, string concatenation
         * is a classic expensive operation.
         * 
         * I haven't actually used any of these, it's just an example.
         */
        protected internal const string TERM_LABEL = "Term";
        protected internal const string TERM_PROPERTY_ID = "id";
        protected internal const string TERM_PROPERTY_RAW = "rawTerm";
        protected internal const string TERM_PROPERTY_LOWER = "lower";

        protected internal const string REL_SUBTERMOF_LABEL = "SUBTERM_OF";



        /// <summary>
        /// Open the connection to the database.
        /// </summary>
        public void open()
        {
            client = new GraphClient(dbLocation);
            client.Connect();
        }

        /// <summary>
        /// Add a new Classifier. Will create a new GLAM if the one provided 
        /// does not currently exist
        /// </summary>
        /// <param name="newClassifier"></param>
        public Classifier addClassifier(Classifier newClassifier)
        {
            this.open();
            if (client != null)
            {
                // Query:
                // MERGE (g:GLAM {name: "glamName"})
                // CREATE (c:Classifier)
                // SET c.email = {email}
                // CREATE (c)-[:ASSOCIATED_WITH]->(g)
                // RETURN c AS addedClassifier
                var query = client.Cypher
                    .Merge("(g:GLAM {name: {glamName} })")
                    .WithParam("glamName", newClassifier.getOrganizationName())
                    .Create("(c:Classifier {email: {email}})")
                    .WithParam("email", newClassifier.email)
                    .Create("(c)-[:ASSOCIATED_WITH]->(g)")
                    .With("c.email as newEmail, g.name AS gName, g.homeUrl AS gUrl")
                    .Return((newEmail, gName, gUrl) => new
                    {
                        classifierEmail = newEmail.As<string>(),
                        glamName = gName.As<string>(),
                        glamUrl = gUrl.As<string>(),
                    })
                    .Results.Single();
                  
                if (query != null)
                {          
                    var rtnGlam = new GLAM(query.glamName);
                    Classifier rtnClassifier = new Classifier(rtnGlam);
                    rtnClassifier.email = query.classifierEmail;
                    return rtnClassifier;
                }
            }
            return null;
        }

        /// <summary>
        /// Get Classifier by email.
        /// </summary>
        /// <param name="email">Classifier's email</param>
        /// <returns>Classifier if exists, else null.</returns>
        public Classifier getClassifier(String email)
        {
            this.open();
            if (client != null)
            {
                // Query
                // MATCH (c:Classifier {email: {email} })
                // RETURN c
                try
                {
                    var query = client.Cypher
                        .Match("(c:Classifier {email: {email} })-[:ASSOCIATED_WITH]->(g:GLAM)")
                        .WithParam("email", email)
                        .With("c.email AS cEmail, g.name AS gName")
                        .Return((cEmail, gName) => new
                        {
                            classifierEmail = cEmail.As<string>(),
                            glamName = gName.As<string>(),
                        }).Results.Single();
                    
                    if (query != null)
                    {
                        var rtnGlam = new GLAM(query.glamName);
                        Classifier rtnClassifier = new Classifier(rtnGlam);
                        rtnClassifier.email = query.classifierEmail;
                        return rtnClassifier;
                    }
                    
                }
                catch (InvalidOperationException)
                {
                    // System.InvalidOperationException: Sequence contains no elements
                }
            }
            return null;
        }

        /// <summary>
        /// Delete a Classifier from the GraphDB.
        /// <para>Mostly for unit testing puposes. Will only delete if has no Classifiables.</para>
        /// </summary>
        public void deleteClassifier(Classifier classifierToDel)
        {
            this.open();
            if (client != null)
            {
                // MATCH (o:Classifier {email: {em} })
                // OPTIONAL MATCH (o)-[r:ASSOCIATED_WITH]->(:GLAM)
                // DELETE o,r
                client.Cypher
                    .Match("(o:Classifier{email: {em} })")
                    .OptionalMatch("(o)-[r:ASSOCIATED_WITH]->(:GLAM)")
                    .WithParam("em", classifierToDel.email)
                    .Delete("o,r")
                    .ExecuteWithoutResults();
            }
        }

        /// <summary>
        /// Might need.
        /// </summary>
        /// <param name="classifierEmail"></param>
        public GLAM getGlamOfClassifier(String classifierEmail)
        {
            return null;
        }

        public List<GLAM> getAllGlams()
        {
            return null;
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

                if (query != null)
                {
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
        /// not been classified. Does not return the owner or concept string.</returns>
        public ClassifiableCollection getAllUnclassified(Classifier classifier)
        {
            ClassifiableCollection resColl = new ClassifiableCollection
            {
                data = new List<Classifiable>(),
            };

            this.open();
            if (client != null)
            {
                // Query 
                // MATCH (c:Classifiable)<-[:OWNS]-(o:Classifier)
                // WHERE c.status = "Unclassified"
                // RETURN c AS classifiable
                // UNION
                // OPTIONAL MATCH (c2:Classifiable)<-[:OWNS]-(:Classifier)-[:ASSOCIATED_WITH]->(g:Glam)
                // WHERE g.name = "US National Parks Service"
                // AND c2.perm = "GLAM"
                // AND c2.status = "Unclassified"
                // RETURN c2 AS classifiable
                var query = client.Cypher
                    .Match("(c:Classifiable)<-[:OWNS]-(o:Classifier)")
                    .Where("c.status = {status}").WithParam("status", Classifiable.Status.Unclassified)
                    .Return((c) => new
                    {
                        classifiable = c.As<Classifiable>(),
                    })
                    .Union()
                    .OptionalMatch("(c2:Classifiable)<-[:OWNS]-(:Classifier)-[:ASSOCIATED_WITH]->(g:GLAM)")
                    .Where("g.name = {classifierGlam}").WithParam("classifierGlam", classifier.getOrganizationName())
                    .AndWhere("c2.perm = {anyonePerm}").WithParam("anyonePerm", Classifiable.Persmission.GLAM)
                    .AndWhere("c2.status = {status}")
                    .Return((c2) => new
                    {
                        classifiable = c2.As<Classifiable>(),
                    })
                    .Results.ToList();

                
                if (query != null)
                {
                    foreach (var res in query)
                    {
                        // if the union has no data, returns as null,
                        // so need to check that we actually have a result
                        if (res.classifiable != null)
                        {
                            res.classifiable.conceptStr = new ConceptString
                            {
                                terms = new List<Term>(),
                            };
                            resColl.data.Add(res.classifiable);
                        }
                    }
                }
            }
            return resColl;
        }

        /// <summary>
        /// Add a new Classifiable to the database. Returns null if 
        /// </summary>
        /// <exception cref="System.NullReferenceException">Thrown when there is insufficient
        /// information for adding a Classifiable.</exception>
        /// <exception cref="Exception">Thrown when not all the Terms in the ConceptString are in the
        /// Classification.</exception>
        /// <param name="newClassifiable">New Classifiable to add. Must have a Classifier.</param>
        /// <returns>The new Classifiable from the Database for verification.</returns>
        public Classifiable addClassifiable(Classifiable newClassifiable)
        {
            // Step 1: Check if there are proper terms
            // TODO: Ummm decide on something else maybe?
            //
            
            if (countNumTermsExist(newClassifiable.conceptStr.terms) != newClassifiable.conceptStr.terms.Count)
            {
                throw new Exception("Some Terms are not in the Classification!");
            }
            
            // Step 2: Go ahead and (try to) add the Classifiable!
            Classifiable rtnClassifiable = new Classifiable();

            this.open();
            if (client != null)
            {
                // Query: Merge classifier based on email, create new Classifiable and 
                // create relationship to the Classifier, split the ConStr into Terms and connect them.
                // MERGE (o:Classifier{email:{em}})
                // ON CREATE SET o.email ={em}
                // CREATE (c:Classifiable {id:{cId}})
                // SET c.name="name-2",c.url = "url2", c.perm="perm2", c.status="status2"
                // CREATE UNQIUE (c)<-[:OWNS]-(o)
                // CREATE UNQIUE (c)-[:HAS_CONSTR]->(cs:Classifiable)
                // SET cs.terms = "(new)(con)(str)"
                // WITH c, cs, REPLACE(cs.terms, "(", "") AS trmStr
                //      UNWIND ( FILTER
                //          ( x in 
                //              SPLIT( trmStr, ")" ) 
                //              WHERE x <> ""
                //          ) 
                //      ) AS t4
                // MATCH (matchedT:Term{rawTerm:t4)})
                // CREATE (cs)-[:HAS_TERM]->(t)
                // WITH c, cs, COLLECT([matchedT.rawTerm]) AS ts
                // RETURN c AS classifiable, ts AS terms
                // NOTE: Owner isn't returned from the actual DB at this point

                // The query is built and executed in stages to check for proper parameters,
                // id is unique, etc.
                var buildQuery = client.Cypher;

                // Try to see if we (generally) have valid non null parmeters
                try
                {
                    buildQuery = buildQuery
                        .WithParams(new
                        {
                            //cId = newClassifiable.owner.getOrganizationName() +
                            //    "_" +
                            //    newClassifiable.name,
                            cId = newClassifiable.id,
                            cName = newClassifiable.name,
                            cUrl = newClassifiable.url,
                            cPerm = newClassifiable.perm,
                            cStatus = newClassifiable.status,
                            em = newClassifiable.owner.email,
                            newConStr = newClassifiable.conceptStr.ToString()
                        });
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine("Classifiable information missing or Classifier email was not set", e);
                    throw new NullReferenceException(@"Classifiable information missing or Classifier email was not set", e);
                }

                // Just throw the exceptions as they happen...?
                buildQuery = buildQuery
                    .Merge("(o:Classifier{email:{em}})")
                    .OnCreate()
                    .Set("o.email ={em}")
                    .Create("(c:Classifiable {id:{cId}})")
                    .Set("c.id = {cId}, c.name = {cName}, c.url = {cUrl}, c.perm = {cPerm}, c.status = {cStatus}")
                    .CreateUnique("(c)<-[:OWNS]-(o)")
                    .CreateUnique("(c)-[:HAS_CONSTR]->(cs:ConceptString)")
                    .Set("cs.terms = {newConStr}");

                // Only go get terms if they exist...
                if (newClassifiable.conceptStr.ToString() != "")
                {
                    buildQuery = buildQuery
                        .With(@"c, cs, REPLACE({newConStr}, ""("", """") AS trmStr
                                UNWIND ( FILTER
                                            ( x in
                                                SPLIT( trmStr, "")"" )
                                                WHERE x <> """"
                                            )
                                        ) AS t4")

                        .Match("(matchedT:Term {rawTerm: t4})")
                        .Create("(cs)-[:HAS_TERM]->(matchedT)")
                        .With("c, COLLECT([matchedT.rawTerm]) AS ts");
                }
                else
                {
                    // If there are no terms, just make a list of strings with only "",
                    // just to prevent writing two essentially the same queries.
                    buildQuery = buildQuery
                        .With("c, COLLECT([\"\"]) AS ts");
                }

                var query = buildQuery.Return((c, ts) => new
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

                    if (query.terms.ElementAt(0) != "")
                    {
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
                    }
                    rtnClassifiable = query.classifiable;
                    rtnClassifiable.owner = newClassifiable.owner;

                    // Reverse for some reason
                    resConStr.terms.Reverse();
                    rtnClassifiable.conceptStr = resConStr;

                    return rtnClassifiable;
                }
            }
            return null;
        }

        /// <summary>
        /// Deletes a classifiable.
        /// </summary>
        /// <param name="classifiable">Classifiable to remove.</param>
        public void deleteClassifiable(Classifiable classifiable)
        {
            this.open();
            if (client != null)
            {
                // MATCH (:Classifier)-[r:OWNS]->(c:Classifiable {id:"Neo4j-dummyiD"})
                // OPTIONAL MATCH (c)-[r2:`HAS_CONSTR`]->(cs:ConceptString)
                // OPTIONAL MATCH (cs)-[r3:HAS_TERM]->(:Term) 
                // DELETE r3, r2, cs, r, c
                client.Cypher
                    .Match("(:Classifier)-[r:OWNS]->(c:Classifiable {id:{cId}})")
                    .OptionalMatch("(c)-[r1:HAS_CONSTR]->(cs:ConceptString)")
                    .OptionalMatch("(cs)-[r2:HAS_TERM]->(:Term)")
                    .WithParam("cId", classifiable.id)
                    .Delete("r2, r1, cs, r, c")
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
        /// <param name="optLimit">Default 25. Should be an integer greater than 0.
        /// Indicate how many results to be returned at max.
        /// Set to 0 if you want all the results with at least one match.</param>
        /// <param name="optSkip">Default 0. Should be a positive integer.
        /// Indicates how many results from the top should be
        /// skipped. Set to 0 if no results should be skipped.</param>
        /// <returns>Returns a ClassifiableCollection where each Classifiable's 
        /// ConceptSring has at least one matching term from </returns>
        public ClassifiableCollection getClassifiablesByConStr(ConceptString conStr,
            int optLimit = 25, int optSkip = 0, bool ordered = false)
        {
            ClassifiableCollection resColl = new ClassifiableCollection
            {
                data = new List<Classifiable>(),
            };

            this.open();

            if (client != null)
            {
                int limit = 25;
                int skip = 0;

                if (optLimit > 0)
                {
                    limit = optLimit;
                }
                if (optSkip > 0)
                {
                    skip = optSkip;
                }

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
                    .Skip(skip)
                    .Limit(limit)
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
        /// Given a list of Terms, return the number of terms that are in
        /// the database.
        /// </summary>
        /// <returns>Number of Terms in the database from the given list.</returns>
        public int countNumTermsExist(List<Term> tList)
        {
            if (tList.Count != 0)
            {
                this.open();

                if (client != null)
                {
                    var query = client.Cypher
                        .Match("(t:Term)")
                        .Where("t.rawTerm = \"\"");

                    for (int i = 0; i < tList.Count; i++)
                    {
                        string tmp = String.Format("t.rawTerm = \"{0}\"", tList[i].rawTerm);
                        query = query.OrWhere(tmp);
                    }

                    var res = query
                        .With("ToInt(COUNT([t])) AS numMatched")
                        .Return((numMatched) => new
                        {
                            counted = numMatched.As<int>(),
                        })
                        .Results.Single();

                    if (query != null)
                    {
                        return res.counted;
                    }
                }
            }
            return 0;
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

        /// <summary>
        /// Adds a new term to the database and recursively adds its children, if any.
        /// </summary>
        /// <param name="newTerm">The new term to add.</param>
        /// <param name="parent">The target parent term, or null if adding to the root.</param>
        /// <returns>The number of nodes added.</returns>
        public int addTerm(Term newTerm, Term parent)
        {
            this.open();

            if (client != null)
            {
                int result = 0;

                if (parent != null)
                {
                    // Cypher can't bind objects with collections, so we need to strip it off the term.
                    Term stripTerm = new Term
                    {
                        id = newTerm.id,
                        rawTerm = newTerm.rawTerm,
                        lower = newTerm.rawTerm
                    };

                    result += client
                        .Cypher
                        .Match("(a:Term)")
                        .Where((Term a) => a.id == parent.id)
                        .Create("(b:Term{addMe})<-[:SUBTERM_OF]-(a)")
                        .WithParam("addMe", newTerm)
                        .Return(() => Return.As<int>("count(b)"))
                        .Results.DefaultIfEmpty(0).FirstOrDefault(); 

                    if (result != 0 && newTerm.subTerms != null)
                    {
                        foreach (Term child in newTerm.subTerms)
                        {
                            result += addTerm(child, newTerm);
                        }
                    }
                }
                else
                {
                    result += _addTermToRoot(newTerm);
                }

                return result;
            }
            else
            { return 0; }
        }

        /// <summary>
        /// Adds a term to BccRoot and recursively adds the terms children, if any.
        /// </summary>
        /// <param name="t"></param>
        /// <returns>The number of nodes added to the database (expect 1, plus one for each child).</returns>
        protected internal int _addTermToRoot(Term t)
        {
            this.open();

            if (client != null)
            {
                int result = 0;

                // Cypher can't bind objects with collections, so we need to strip it off the term.
                Term stripTerm = new Term();
                stripTerm.id = t.id;
                stripTerm.rawTerm = t.rawTerm;
                stripTerm.lower = t.lower;

                result += client.Cypher
                        .Match("(a:BccRoot)")
                        .Where("a.id = \"bccRoot\"")
                        .Create("(:Term{addMe})-[r:SUBTERM_OF]->(a)")
                        .WithParam("addMe", stripTerm)
                        .Return(() => Return.As<int>("count(r)"))
                        .Results.DefaultIfEmpty(0).FirstOrDefault();

                if (result == 0)
                {
                    // Something fucky happened.
                    Console.Error.WriteLine("In Neo4jDB._addTermToRoot, the added term could not be linked to the root node.");
                }
                else if (t.subTerms != null)
                {
                    foreach (Term child in t.subTerms)
                    {
                        result += addTerm(child, t);
                    }
                }

                return result;
            }
            else
            { return 0; }
        }

        /// <summary>
        /// Move a term from one parent to another. If either the target or the new parent don't exist in the database, this operation has no effect.
        /// </summary>
        /// <param name="target">The term to move</param>
        /// <param name="newParent">The new target parent</param>
        /// <returns>The number of nodes affected by the operation. You should expect a typical result of 2 (one relationship deleted, and one created).</returns>
        public int moveTerm(Term target, Term newParent)
        {
            this.open();

            if (client != null)
            {
                // Ensure that both the target and the new parent exist in the DB
                var targetSearch = client.Cypher
                    .Match("(a:Term{id:{PARAM1}})")
                    .WithParam("PARAM1", target.id)
                    .Return(() => Return.As<int>("count(a)"))
                    .Results.DefaultIfEmpty(0).FirstOrDefault();
                if (targetSearch != 1) return 0;

                var newParentSearch = client.Cypher
                    .Match("(a:Term{id:{PARAM1}})")
                    .WithParam("PARAM1", newParent.id)
                    .Return(() => Return.As<int>("count(a)"))
                    .Results.DefaultIfEmpty(0).FirstOrDefault();
                if (newParentSearch != 1) return 0;

                // Delete the old relationship
                var deleteResult = client
                    .Cypher
                    .Match("(a:Term{id:{Param_ID}})-[r:SUBTERM_OF]-()")
                    .WithParam("Param_ID", (target.id))
                    .Delete("r")
                    .Return(() => Return.As<int>("count(r)"))
                    .Results.DefaultIfEmpty(0).FirstOrDefault();

                // Create the new relationship
                var createResult = client
                    .Cypher
                    .Match("(a:Term), (b:Term)")
                    .Where((Term a) => a.id == target.id)
                    .AndWhere((Term b) => b.id == newParent.id)
                    .Create("(a)-[r:SUBTERM_OF]->(b)")
                    .Return(() => Return.As<int>("count(r)"))
                    .Results.DefaultIfEmpty(0).FirstOrDefault();

                return deleteResult + createResult;
            }
            else { return 0; }
        }

        /// <summary>
        /// Safe-deletes a term from the database. The operation has no effect if the target has child terms.
        /// </summary>
        /// <param name="t">A term object representing the term to be added. Note that ID is the only matching criteria.</param>
        /// <returns>The number of nodes affected by the operation. This will include:
        ///     (a) The deleted node
        ///     (b) Concept strings containing the deleted term
        ///     (c) Classifiables with concept strings that contained the deleted term
        /// </returns>
        public int delTerm(Term t)
        {
            return 0;

            // TODO
        }

        /// <summary>
        /// Force-delete a given term and all relationships attached to it - that is, the term and all its relationships are deleted and no checking is done.
        /// </summary>
        /// <param name="t">The term to be deleted. Note that the only matching criteria is id.</param>
        /// <returns>The number of nodes and the number of relationships deleted.</returns>
        public int delTermFORCE(Term t)
        {
            this.open();

            if (client != null)
            {
                return client
                    .Cypher
                    .Match("(a:Term{id:{Param_ID}})-[r]-(), (b:Term{id:{Param_ID}})")
                    .WithParam("Param_ID", (t.id))
                    .Delete("a, b, r")
                    .Return(() => Return.As<int>("count(b) + count(r)"))
                    .Results.DefaultIfEmpty(0).FirstOrDefault();
            }
            else
            {
                return 0;
            }
        }
    }
}