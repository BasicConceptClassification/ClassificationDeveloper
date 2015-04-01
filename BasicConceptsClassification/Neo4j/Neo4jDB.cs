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

        // For getRecentlyClassified, etc, if for some reason they don't exist...
        protected internal const string UNKNOWN_GLAM = "Unknown GLAM";
        protected internal const string UNKNOWN_OWNER_EMAIL = "Unknown Owner Email";
        protected internal const string UNKNOWN_OWNER_USERNAME = "Unknown Owner Username";
        protected internal const string UNKNOWN_EDITOR_EMAIL = "Unknown Last Edited Email";
        protected internal const string UNKNOWN_EDITOR_USERNAME = "Unknown Last Edited Username";
        

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
        /// does not currently exist.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the Classifier's name or email is null.</exception>
        /// <param name="newClassifier">Returns the new Classifier.</param>
        public Classifier addClassifier(Classifier newClassifier)
        {
            // Argument checking. Throw an exception if there's a problem
            if (newClassifier.email == null || newClassifier.email == "")
            {
                throw new ArgumentNullException("Classifier's email is not set.", "newClassifier.email");
            }
            if (newClassifier.username == null || newClassifier.username == "")
            {
                throw new ArgumentNullException("Classifier's username is not set.", "newClassifier.name");
            }

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
                    .Set("c.username = {username}").WithParam("username", newClassifier.username)
                    .Create("(c)-[:ASSOCIATED_WITH]->(g)")
                    .With("c.email as newEmail, c.username as newName, g.name AS gName")
                    .Return((newEmail, newName, gName) => new
                    {
                        classifierEmail = newEmail.As<string>(),
                        classifierName = newName.As<string>(),
                        glamName = gName.As<string>(),
                    })
                    .Results.Single();

                if (query != null)
                {
                    var rtnGlam = new GLAM(query.glamName);
                    Classifier rtnClassifier = new Classifier(rtnGlam);
                    rtnClassifier.email = query.classifierEmail;
                    rtnClassifier.username = query.classifierName;
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
                        .With("c.email AS cEmail, c.username as cName, g.name AS gName")
                        .Return((cEmail, cName, gName) => new
                        {
                            classifierEmail = cEmail.As<string>(),
                            classifierName = cName.As<string>(),
                            glamName = gName.As<string>(),
                        }).Results.Single();

                    if (query != null)
                    {
                        var rtnGlam = new GLAM(query.glamName);
                        Classifier rtnClassifier = new Classifier(rtnGlam);
                        rtnClassifier.email = query.classifierEmail;
                        rtnClassifier.username = query.classifierName;
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
            this.open();
            if (client != null)
            {
                // Query
                // MATCH (g:GLAM)
                // WITH g.name as gName
                // ORDER BY gName
                // RETURN gName
                var query = client.Cypher
                    .Match("(g:GLAM)<-[:ASSOCIATED_WITH]-(c:Classifier {email:{em}})")
                    .WithParam("em", classifierEmail)
                    .With("g.name AS gName")
                    .Return((gName) => new
                    {
                        glamName = gName.As<string>(),
                    }).Results.Single();

                if (query != null)
                {
                    return new GLAM(query.glamName);
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves the list of all GLAMs.
        /// </summary>
        /// <param name="alphabetical">Sorts alphabetically by default.</param>
        /// <returns></returns>
        public List<GLAM> getAllGlams(bool alphabetical = true)
        {
            List<GLAM> rtnGLAMs = new List<GLAM>();

            this.open();
            if (client != null)
            {
                // Query
                // MATCH (g:GLAM)
                // WITH g.name as gName
                // ORDER BY gName
                // RETURN gName
                var query = client.Cypher
                    .Match("(g:GLAM)")
                    .With("g.name AS gName");
                if (alphabetical)
                {
                    query = query.OrderBy("gName");
                }
                var result = query.Return((gName) => new
                    {
                        glamName = gName.As<string>(),
                    }).Results.ToList();

                if (result != null)
                {
                    foreach (var res in result)
                    {
                        GLAM tmpGLAM = new GLAM(res.glamName);
                        rtnGLAMs.Add(tmpGLAM);
                    }
                }
            }
            return rtnGLAMs;
        }

        /// <summary>
        /// For Testing.
        /// </summary>
        /// <param name="glam"></param>
        public void deleteGlam(GLAM glam)
        {
            this.open();
            if (client != null)
            {
                // MATCH (o:Classifier {email: {em} })
                // OPTIONAL MATCH (o)-[r:ASSOCIATED_WITH]->(:GLAM)
                // DELETE o,r
                client.Cypher
                    .Match("(g:GLAM{name: {name} })")
                    .WithParam("name", glam.name)
                    .Delete("g")
                    .ExecuteWithoutResults();
            }
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
                // Match (g1:GLAM)<-[:ASSOCIATED_WITH]-(owner:Classifier)-[:OWNS]->(c)
                // OPTIONAL MATCH (cs)-[:HAS_TERM]->(t:Term) 
                // OPTIONAL MATCH (g2:GLAM)<-[:ASSOCIATED_WITH]-(lastEditor:Classifier)-[:MODIFIED_BY]->(c)
                // RETURN 	c, 
                //          COLLECT ([t]) as ts,
                //          
                // We have g and g2 for the two GLAMs in case another user outside of the GLAM
                // is allowed to modify...like the admin?
                var query = client.Cypher
                    .Match("(c:Classifiable{id:{id}})-[:HAS_CONSTR]->(cs)")
                    .WithParam("id", id)
                    .Match("(g1:GLAM)<-[:ASSOCIATED_WITH]-(owner:Classifier)-[:OWNS]->(c)")
                    .OptionalMatch("(cs)-[:HAS_TERM]->(t:Term)")
                    .OptionalMatch("(g2:GLAM)<-[:ASSOCIATED_WITH]-(lastEditor:Classifier)-[:MODIFIED_BY]->(c)")
                    .With(@"c, t, g1.name AS ownerG, g2.name AS editorG, 
                            owner.email AS ownerE, owner.username AS ownerN,
                            lastEditor.email AS editorE, lastEditor.username AS editorN")
                    .Return((c, t, ownerG, ownerE, ownerN, editorG, editorE, editorN) => new
                    {
                        classifiable = c.As<Classifiable>(),
                        terms = t.CollectAs<Term>(),
                        ownerGlam = ownerG.As<string>(),
                        ownerEmail = ownerE.As<string>(),
                        ownerName = ownerN.As<string>(),
                        editorGlam = editorG.As<string>(),
                        editorEmail = editorE.As<string>(),
                        editorName = editorN.As<string>(),
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
                    //resConStr.terms.Reverse();
                    query.classifiable.conceptStr = resConStr;

                    // If these are not null...
                    if (query.ownerGlam != null && query.ownerEmail != null && query.ownerName != null)
                    {
                        GLAM tmpG = new GLAM(query.ownerGlam);
                        query.classifiable.owner = new Classifier(tmpG);
                        query.classifiable.owner.email = query.ownerEmail;
                        query.classifiable.owner.username = query.ownerName;
                    }
                    else
                    {
                        GLAM tmpG = new GLAM(UNKNOWN_GLAM);
                        query.classifiable.owner = new Classifier(tmpG);
                        query.classifiable.owner.email = UNKNOWN_OWNER_EMAIL;
                        query.classifiable.owner.username = UNKNOWN_OWNER_USERNAME;
                    }

                    // If these two are not null...
                    if (query.editorGlam != null && query.editorEmail != null && query.editorName != null)
                    {
                        GLAM tmpG = new GLAM(query.editorGlam);
                        query.classifiable.classifierLastEdited = new Classifier(tmpG);
                        query.classifiable.classifierLastEdited.email = query.editorEmail;
                        query.classifiable.classifierLastEdited.username = query.editorName;
                    }
                    else
                    {
                        GLAM tmpG = new GLAM(UNKNOWN_GLAM);
                        query.classifiable.classifierLastEdited = new Classifier(tmpG);
                        query.classifiable.classifierLastEdited.email = UNKNOWN_EDITOR_EMAIL;
                        query.classifiable.classifierLastEdited.username = UNKNOWN_EDITOR_USERNAME;
                    }
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
                // MATCH (c:`Classifiable`)
                // WHERE c.name = {name}
                // OPTIONAL MATCH (c)-[:HAS_CONSTR]->(cs:ConceptString)-[:HAS_TERM]->(t:Term) 
                // RETURN 	c, 
                //          COLLECT ([t]) as ts
                var query = client.Cypher
                    .Match("(c:Classifiable)")
                    .Where("c.name = {name}").WithParam("name", name)
                    .OptionalMatch("(c)-[:HAS_CONSTR]->(cs:ConceptString)-[:HAS_TERM]->(t:Term)")
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
        /// Given a letter of the alphabet, will return all Classifiables that start
        /// with that letter. Is case insensitive.
        /// </summary>
        /// <param name="letter">If the letter provided is not A-Z or a-z then it will
        /// fetch all classifiables that do not start with A-Z or a-z.</param>
        /// <returns>ClassifiableCollection that starts with that letter, in alphabetical order.</returns>
        public ClassifiableCollection getClassifiablesByAlphaGroup(char letter)
        {

            ClassifiableCollection rtnColl = new ClassifiableCollection
            {
                data = new List<Classifiable>(),
            };

            this.open();
            if (client != null)
            {
                // Query: Optional match for the classifier just in case it somehow ends up as a stray
                // MATCH (c:Classifiable) 
                // OPTIONAL MATCH (g1:GLAM)<-[:ASSOCIATED_WITH]-(owner:Classifier)-[:OWNS]->(c)
                // Where c.name =~ "[Aa].*"
                // RETURN c.name AS name, c, t, g1.name AS ownerG, g2.name AS editorG, 
                //         owner.email AS ownerE, owner.username AS ownerN
                // ORDER BY name

                // Need to do the formatting outside of the query for the pattern matching in the query.
                string regex = "";
                if (Char.IsLetter(letter))
                {
                    regex = String.Format("[{0}{1}].*", char.ToUpper(letter), char.ToLower(letter));
                }
                else
                {
                    regex = "[^A-Za-z].*";
                }

                var query = client.Cypher
                    .Match("(c:Classifiable)")
                    .Where("c.name =~ {re}").WithParam("re", regex)
                    .OptionalMatch("(c)-[:HAS_CONSTR]->(cs:ConceptString)-[:HAS_TERM]->(t:Term)")
                    .OptionalMatch("(g1:GLAM)<-[:ASSOCIATED_WITH]-(owner:Classifier)-[:OWNS]->(c)")
                    .With(@"c.name AS name, c, t, g1.name AS ownerG, 
                            owner.email AS ownerE, owner.username AS ownerN")
                    .Return((c, t, ownerG, ownerE, ownerN) => new
                    {
                        classifiable = c.As<Classifiable>(),
                        terms = t.CollectAs<Term>(),
                        ownerGlam = ownerG.As<string>(),
                        ownerEmail = ownerE.As<string>(),
                        ownerName = ownerN.As<string>(),
                    }).OrderBy("c.name")
                    .Results.ToList();


                if (query != null)
                {
                    foreach (var res in query)
                    {
                        // Build the concept string
                        ConceptString resConStr = new ConceptString
                        {
                            terms = new List<Term>(),
                        };

                        // Get the terms from the concept string
                        foreach (var t in res.terms)
                        {
                            t.Data.subTerms = new List<Term>();
                            resConStr.terms.Add(t.Data);
                        }

                        // Maintains order probably because it retained the added order, then add the concept string
                        resConStr.terms.Reverse();
                        res.classifiable.conceptStr = resConStr;

                        // Add the owner if the information can be found. Could be possible that some stray
                        // Classifiable exists
                        if (res.ownerGlam != null && res.ownerEmail != null && res.ownerName != null)
                        {
                            GLAM tmpG = new GLAM(res.ownerGlam);
                            res.classifiable.owner = new Classifier(tmpG);
                            res.classifiable.owner.email = res.ownerEmail;
                            res.classifiable.owner.username = res.ownerName;
                        }
                        else
                        {
                            GLAM tmpG = new GLAM(UNKNOWN_GLAM);
                            res.classifiable.owner = new Classifier(tmpG);
                            res.classifiable.owner.email = UNKNOWN_OWNER_EMAIL;
                            res.classifiable.owner.username = UNKNOWN_OWNER_USERNAME;
                        }

                        // Add Classifiable to the collection
                        rtnColl.data.Add(res.classifiable);
                    }
                }
            }
            return rtnColl;
        }

        /// <summary>
        /// Get all the Classifiables that the classifier owns in a ClassifiableCollection.
        /// </summary>
        /// <param name="owner">The owner of the Classifiables</param>
        /// <returns>ClassifiableCollection of the Classifier's Classifiables.</returns>
        public ClassifiableCollection getOwnedClassifiables(Classifier owner)
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
                // WHERE o.email = "testingRecent@BCCNeo4j.com"
                // OPTIONAL MATCH (c)-[:HAS_CONSTR]->(cs)-[:HAS_TERM]->(t:Term)
                // RETURN c AS classifiable
                // ORDER BY c.name
                var query = client.Cypher
                    .Match("(c:Classifiable)<-[:OWNS]-(owner:Classifier)")
                    .Where("owner.email = {email}").WithParam("email", owner.email)
                    .OptionalMatch("(c)-[:HAS_CONSTR]->(cs)-[:HAS_TERM]->(t:Term)")
                    .Return((c, t) => new
                    {
                        classifiable = c.As<Classifiable>(),
                        terms = t.CollectAs<Term>(),
                    })
                    .OrderBy("c.name")
                    .Results.ToList();

                if (query != null)
                {
                    foreach (var res in query)
                    {
                        // if the union has no data, returns as null,
                        // so need to check that we actually have a result
                        if (res.classifiable != null)
                        {
                            // TODO: reorder terms to match concept string
                            ConceptString resConStr = new ConceptString
                            {
                                terms = new List<Term>(),
                            };

                            // Thanks muchy to the example:
                            // https://github.com/neo4j-contrib/developer-resources/blob/gh-pages/language-guides/dotnet/neo4jclient/Neo4jDotNetDemo/Controllers/MovieController.cs

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
            }
            return resColl;
        }

        /// <summary>
        /// Get the classifier's recently classified classifiables.
        /// </summary>
        /// <param name="classifierEmail">Classifier who owns the classifiables returned.</param>
        /// <param name="limit">Limit number of results. Default 10. If set to a number 
        /// greater than 0, then it will limit to that many results. 0 or less, and it will grab them all.</param>
        /// <returns>Classifiables without their concept string, owner, who recently classified them,
        /// etc.</returns>
        public ClassifiableCollection getRecentlyClassified(string classifierEmail, int limit = 10)
        {
            ClassifiableCollection resColl = new ClassifiableCollection
            {
                data = new List<Classifiable>(),
            };

            this.open();
            if (client != null)
            {
                // Query 
                // MATCH (c)<-[rModify:MODIFIED_BY]-(o)
                // WHERE o.email = "testingRecent@BCCNeo4j.com"
                // RETURN c AS classifiable, rModify.lastModified as date
                // ORDER BY date
                var queryBuild = client.Cypher
                    .Match("(c:Classifiable)<-[rModified:MODIFIED_BY]-(o:Classifier)-[:ASSOCIATED_WITH]->(g1:GLAM)")
                    .Where("o.email = {email}").WithParam("email", classifierEmail)
                    .AndWhere("c.status = {classed}").WithParam("classed", Classifiable.Status.Classified.ToString())
                    .OptionalMatch("(c)-[:HAS_CONSTR]->(cs:ConceptString)")
                    .OptionalMatch("(cs)-[:HAS_TERM]->(t:Term)")
                    .OptionalMatch("(g2:GLAM)<-[:ASSOCIATED_WITH]-(lastEditor:Classifier)-[:MODIFIED_BY]->(c)")
                    .With(@"c, t, rModified.lastModified AS date,
                            g1.name AS ownerG, o.email AS ownerE, o.username AS ownerN,
                            g2.name AS editorG, lastEditor.email AS editorE, lastEditor.username AS editorN")
                    
                    .Return((c, t, ownerG, ownerE, ownerN, editorG, editorE, editorN, date) => new
                    {
                        classifiable = c.As<Classifiable>(),
                        terms = t.CollectAs<Term>(),
                        ownerGlam = ownerG.As<string>(),
                        ownerEmail = ownerE.As<string>(),
                        ownerName = ownerN.As<string>(),
                        editorGlam = editorG.As<string>(),
                        editorEmail = editorE.As<string>(),
                        editorName = editorN.As<string>(),
                        date = date.As<long>(),
                    })
                    .OrderByDescending("date");

                if (limit > 0){
                    queryBuild = queryBuild.Limit(limit);
                }

                var query = queryBuild
                    .Results.ToList();

                if (query != null)
                {
                    foreach (var res in query)
                    {
                        // if one set of union has no data, it will return as null,
                        // so need to check that we actually have results from each set
                        if (res.classifiable != null)
                        {
                            // TODO: reorder terms to match concept string
                            ConceptString resConStr = new ConceptString
                            {
                                terms = new List<Term>(),
                            };

                            foreach (var t in res.terms)
                            {
                                t.Data.subTerms = new List<Term>();
                                resConStr.terms.Add(t.Data);
                            }

                            // A bit of a hack for now. But it maintains order because of
                            // ...some reason.
                            resConStr.terms.Reverse();
                            res.classifiable.conceptStr = resConStr;

                            // If these are not null...
                            if (res.ownerGlam != null && res.ownerEmail != null && res.ownerName != null)
                            {
                                GLAM tmpG = new GLAM(res.ownerGlam);
                                res.classifiable.owner = new Classifier(tmpG);
                                res.classifiable.owner.email = res.ownerEmail;
                                res.classifiable.owner.username = res.ownerName;
                            }
                            else
                            {
                                GLAM tmpG = new GLAM(UNKNOWN_GLAM);
                                res.classifiable.owner = new Classifier(tmpG);
                                res.classifiable.owner.email = UNKNOWN_OWNER_EMAIL;
                                res.classifiable.owner.username = UNKNOWN_OWNER_USERNAME;
                            }

                            // If these two are not null...
                            if (res.editorGlam != null && res.editorEmail != null && res.editorName != null)
                            {
                                GLAM tmpG = new GLAM(res.editorGlam);
                                res.classifiable.classifierLastEdited = new Classifier(tmpG);
                                res.classifiable.classifierLastEdited.email = res.editorEmail;
                                res.classifiable.classifierLastEdited.username = res.editorName;
                            }
                            else
                            {
                                GLAM tmpG = new GLAM(UNKNOWN_GLAM);
                                res.classifiable.classifierLastEdited = new Classifier(tmpG);
                                res.classifiable.classifierLastEdited.email = UNKNOWN_EDITOR_EMAIL;
                                res.classifiable.classifierLastEdited.username = UNKNOWN_EDITOR_USERNAME;
                            }

                            resColl.data.Add(res.classifiable);
                        }
                    }
                }
            }
            return resColl;
        }

        /// <summary>
        /// Gets any classifiables that the Classifier is allow to classify.
        /// </summary>
        /// <param name="classifierEmail"></param>
        /// <param name="filterByStatus">Can be set to Classified, Unclassified, to narrow down
        /// the results.</param>
        /// <returns></returns>
        public ClassifiableCollection getAllowedClassifiables(string classifierEmail, string filterByStatus = "All")
        {
            ClassifiableCollection resColl = new ClassifiableCollection
            {
                data = new List<Classifiable>(),
            };

            bool filter = true;
            if (filterByStatus != Classifiable.Status.Classified.ToString() &&
                 filterByStatus != Classifiable.Status.Unclassified.ToString())
            {
                filter = false;
            }

            this.open();
            if (client != null)
            {
                // Query 
                // MATCH (c:Classifiable)<-[:OWNS]-(o:Classifier)
                // WHERE o.email = {email}
                //     AND c.status = {filterByStatus}
                // RETURN c AS classifiable
                // UNION
                // OPTIONAL MATCH (c2:Classifiable)<-[:OWNS]-(:Classifier)-[:ASSOCIATED_WITH]->(g:Glam)
                // WHERE g.name = "US National Parks Service"
                // AND c2.perm = "GLAM"
                //     AND c2.status = {filterByStatus}
                // RETURN c2 AS classifiable
                var query = client.Cypher
                    .Match("(c:Classifiable)<-[:OWNS]-(o:Classifier)-[:ASSOCIATED_WITH]->(g1:GLAM)")
                    .Where("o.email = {email}").WithParam("email", classifierEmail);

                // If we're filtering by status, do it here
                if (filter == true)
                {
                    query = query.AndWhere("c.status = {status}").WithParam("status", filterByStatus);
                }

                // FInd the concept string, terms in it (if any), who last modified it.
                // Then return everything.
                query = query
                    .OptionalMatch("(c)-[:HAS_CONSTR]->(cs:ConceptString)")
                    .OptionalMatch("(cs)-[:HAS_TERM]->(t:Term)")
                    .OptionalMatch("(g2:GLAM)<-[:ASSOCIATED_WITH]-(lastEditor:Classifier)-[:MODIFIED_BY]->(c)")
                    .With(@"c, t, 
                            g1.name AS ownerG, o.email AS ownerE, o.username AS ownerN,
                            g2.name AS editorG, lastEditor.email AS editorE, lastEditor.username AS editorN")
                    .Return((c, t, ownerG, ownerE, ownerN, editorG, editorE, editorN) => new
                    {
                        classifiable = c.As<Classifiable>(),
                        terms = t.CollectAs<Term>(),
                        ownerGlam = ownerG.As<string>(),
                        ownerEmail = ownerE.As<string>(),
                        ownerName = ownerN.As<string>(),
                        editorGlam = editorG.As<string>(),
                        editorEmail = editorE.As<string>(),
                        editorName = editorN.As<string>(),
                    })
                    .Union()
                    .Match("(c2:Classifiable)<-[:OWNS]-(o2:Classifier)-[:ASSOCIATED_WITH]->(g1_2:GLAM)<-[:ASSOCIATED_WITH]-(oAgain:Classifier)")
                    .Where("oAgain.email = {emailAgain}").WithParam("emailAgain", classifierEmail)
                    .AndWhere("c2.perm = {anyonePerm}").WithParam("anyonePerm", Classifiable.Permission.GLAM)
                    .AndWhere("o2.email <> {email}");

                if (filter == true)
                {
                    query = query.AndWhere("c2.status = {status}");
                }

                // FInd the concept string, terms in it (if any), who last modified it.
                // Then return everything.
                var queryRes = query
                    .OptionalMatch("(c2)-[:HAS_CONSTR]->(cs2:ConceptString)")
                    .OptionalMatch("(cs2)-[:HAS_TERM]->(t2:Term)")
                    .OptionalMatch("(g2_2:GLAM)<-[:ASSOCIATED_WITH]-(lastEditor2:Classifier)-[:MODIFIED_BY]->(c2)")
                    .With(@"c2, t2, 
                            g1_2.name AS ownerG2, oAgain.email AS ownerE2, oAgain.username AS ownerN2,
                            g2_2.name AS editorG2, lastEditor2.email AS editorE2, lastEditor2.username AS editorN2")
                    .Return((c2, t2, ownerG2, ownerE2, ownerN2, editorG2, editorE2, editorN2) => new
                    {
                        classifiable = c2.As<Classifiable>(),
                        terms = t2.CollectAs<Term>(),
                        ownerGlam = ownerG2.As<string>(),
                        ownerEmail = ownerE2.As<string>(),
                        ownerName = ownerN2.As<string>(),
                        editorGlam = editorG2.As<string>(),
                        editorEmail = editorE2.As<string>(),
                        editorName = editorN2.As<string>(),
                    })
                    .Results.ToList();


                if (queryRes != null)
                {
                    foreach (var res in queryRes)
                    {
                        // if one set of union has no data, it will return as null,
                        // so need to check that we actually have results from each set
                        if (res.classifiable != null)
                        {
                            // TODO: reorder terms to match concept string
                            ConceptString resConStr = new ConceptString
                            {
                                terms = new List<Term>(),
                            };

                            // Thanks muchy to the example:
                            // https://github.com/neo4j-contrib/developer-resources/blob/gh-pages/language-guides/dotnet/neo4jclient/Neo4jDotNetDemo/Controllers/MovieController.cs

                            foreach (var t in res.terms)
                            {
                                t.Data.subTerms = new List<Term>();
                                resConStr.terms.Add(t.Data);
                            }

                            // A bit of a hack for now. But it maintains order because of
                            // ...some reason.
                            resConStr.terms.Reverse();
                            res.classifiable.conceptStr = resConStr;

                            // If these are not null...
                            if (res.ownerGlam != null && res.ownerEmail != null && res.ownerName != null)
                            {
                                GLAM tmpG = new GLAM(res.ownerGlam);
                                res.classifiable.owner = new Classifier(tmpG);
                                res.classifiable.owner.email = res.ownerEmail;
                                res.classifiable.owner.username = res.ownerName;
                            }
                            else
                            {
                                GLAM tmpG = new GLAM(UNKNOWN_GLAM);
                                res.classifiable.owner = new Classifier(tmpG);
                                res.classifiable.owner.email = UNKNOWN_OWNER_EMAIL;
                                res.classifiable.owner.username = UNKNOWN_OWNER_USERNAME;
                            }

                            // If these two are not null...
                            if (res.editorGlam != null && res.editorEmail != null && res.editorName != null)
                            {
                                GLAM tmpG = new GLAM(res.editorGlam);
                                res.classifiable.classifierLastEdited = new Classifier(tmpG);
                                res.classifiable.classifierLastEdited.email = res.editorEmail;
                                res.classifiable.classifierLastEdited.username = res.editorName;
                            }
                            else
                            {
                                GLAM tmpG = new GLAM(UNKNOWN_GLAM);
                                res.classifiable.classifierLastEdited = new Classifier(tmpG);
                                res.classifiable.classifierLastEdited.email = UNKNOWN_EDITOR_EMAIL;
                                res.classifiable.classifierLastEdited.username = UNKNOWN_EDITOR_USERNAME;
                            }

                            resColl.data.Add(res.classifiable);
                        }
                    }
                }
            }
            return resColl;
        }

        /// <summary>
        /// Get all the Classifiables that have the status "Unclassified" that the Classifier is allowed to
        /// classify.
        /// </summary>
        /// <returns>A ClassifiableCollection with Classifiables that have
        /// not been classified.</returns>
        public ClassifiableCollection getAllUnclassified(string classifierEmail)
        {
            return getAllowedClassifiables(classifierEmail, Classifiable.Status.Unclassified.ToString());
        }

        /// <summary>
        /// Get all the Classifiables that have the status "Classifiable" that the Classifier is allowed to
        /// classify.
        /// </summary>
        /// <returns>A ClassifiableCollection with Classifiables that have
        /// been classified.</returns>
        public ClassifiableCollection getAllClassified(string classifierEmail)
        {
            return getAllowedClassifiables(classifierEmail, Classifiable.Status.Classified.ToString());
        }

        /// <summary>
        /// Add a new Classifiable to the database. Returns null if 
        /// </summary>
        /// <exception cref="System.NullReferenceException">Thrown when there is insufficient
        /// information for adding a Classifiable.</exception>
        /// <exception cref="System.ArgumentException">1) Thrown when not all the Terms in the ConceptString are in the
        /// Classification.
        /// <para>2) Thrown when the the id is not unique.</para></exception>
        /// <param name="newClassifiable">New Classifiable to add. Must have a Classifier.</param>
        /// <returns>The new Classifiable from the Database for verification.</returns>
        public Classifiable addClassifiable(Classifiable newClassifiable)
        {
            // Check 1: Check if there are proper terms
            if (countNumTermsExist(newClassifiable.conceptStr.terms) != newClassifiable.conceptStr.terms.Count)
            {
                throw new System.ArgumentException("Some Terms are not in the Classification!",
                    "Classifiable.conceptStr");
            }

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
                // WITH c.id as cId
                // RETURN cId
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
                            cId = newClassifiable.owner.getOrganizationName() +
                                "_" +
                                newClassifiable.name,
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
                    .Set("o.email ={em}");

                // UNIQUE Id
                try
                {
                    buildQuery = buildQuery
                        .Create("(c:Classifiable {id:{cId}})");
                }
                catch (NeoException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.NeoMessage);
                    throw new System.ArgumentException(ex.NeoMessage, "Classifiable.name");
                }

                buildQuery = buildQuery
                    .Set("c.id = {cId}, c.name = {cName}, c.url = {cUrl}, c.perm = {cPerm}, c.status = {cStatus}")
                    .CreateUnique("(c)<-[:OWNS]-(o)")
                    .CreateUnique("(c)<-[rModify:MODIFIED_BY]-(o)")
                    .Set("rModify.lastModified = timestamp()")
                    .CreateUnique("(c)-[:HAS_CONSTR]->(cs:ConceptString)")
                    .Set("cs.terms = {newConStr}");

                // Only create relationships to terms if they exist
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
                        .Create("(cs)-[:HAS_TERM]->(matchedT)");
                }
                try
                {
                    var query = buildQuery
                        .With("c.id as newId")
                        .Return((newId) => new
                        {
                            cId = newId.As<string>(),
                        }).Results.FirstOrDefault();

                    if (query != null)
                    {
                        return getClassifiableById(query.cId);
                    }
                }
                catch (NeoException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.NeoMessage);
                    throw new System.ArgumentException(ex.NeoMessage, "Classifiable.name");
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
                // OPTIONAL MATCH (c)-[rOther]-()
                // DELETE r3, r2, cs, r, c, rOther
                client.Cypher
                    .Match("(:Classifier)-[r:OWNS]->(c:Classifiable {id:{cId}})")
                    .OptionalMatch("(c)-[r1:HAS_CONSTR]->(cs:ConceptString)")
                    .OptionalMatch("(cs)-[r2:HAS_TERM]->(:Term)")
                    .OptionalMatch("(c)-[rOther]-()")
                    .WithParam("cId", classifiable.id)
                    .Delete("r2, r1, cs, r, c, rOther")
                    .ExecuteWithoutResults();
            }
        }

        /// <summary>
        /// Given an old classifiable and the updated version, will update the old classifiable to the new one.
        /// </summary>
        /// <param name="oldClassifiable">The old information.</param>
        /// <param name="updatedClassifiable">The updated information.</param>
        /// <param name="modifier">The Classifier who modified the Classifiable.</param>
        /// <exception cref="ArgumentException">Thrown when not all the terms in the updated
        /// Classifiable are in the Classification.</exception>
        /// <returns>The classifiable with the basic updated information 
        /// and concept string (no owner information).</returns>
        public Classifiable updateClassifiable(Classifiable oldClass, Classifiable updatedClass, Classifier modifier)
        {
            // Check 1: Check if there are proper terms
            // TODO: Ummm decide on something else maybe?
            if (countNumTermsExist(updatedClass.conceptStr.terms) != updatedClass.conceptStr.terms.Count)
            {
                throw new ArgumentException("Some Terms are not in the Classification!", "updatedClass.conceptStr");
            }

            // Try to update
            this.open();
            if (client != null)
            {
                // Query: Merge classifier based on email, create new Classifiable and 
                // create relationship to the Classifier, split the ConStr into Terms and connect them.
                //
                // MATCH (c:Classifiable {id: "Updating GLAM_dummyName1" })<-[:OWNS]-(owner:Classifier)-[:ASSOCIATED_WITH]->(g:GLAM)
                // SET c.id = g.name + "_newName"
                // SET c.name = "newName", c.url = "newUrl", c.perm = "OwnerOnly", c.status = "Unclassified"
                // WITH c
                // MATCH (c)<-[rModify:MODIFIED_BY]-(prevClassifier:Classifier)
                // DELETE rModify
                // With c
                // Match(recentClassifier:Classifier {email: "testingUpdateSimple@BCCNeo4j.com" })
                // CREATE UNIQUE (c)<-[rNewModify:MODIFIED_BY]-(recentClassifier)
                // SET rNewModify.lastModified = timestamp()
                // RETURN c.id AS cId
                // NOTE: Owner isn't returned from the actual DB at this point

                // The query is built and executed in stages to check for proper parameters,
                // id is unique, etc.
                var buildQuery = client.Cypher;

                //Check 2?
                // TODO: a way to make sure the editing classifier has permission to edit this Classifiable?

                // Update 1) Set the updated basic information (id, name, url, perm, status)
                // Update 2) Update who last modified this. Could be done at the end, but got 
                // moved up here when there was some multiple term editing weirdness. Wanted
                // to grouo the 'easy' updating stuff together
                buildQuery = buildQuery
                    .Match("(c:Classifiable {id: {oldId} })<-[:OWNS]-(:Classifier)-[:ASSOCIATED_WITH]->(g:GLAM)")
                    .WithParam("oldId", oldClass.id)
                    .Set("c.id = g.name + {updatedId}").WithParam("updatedId", "_" + updatedClass.name)
                    .Set("c.name = {upName}, c.url = {upUrl}, c.perm = {upPerm}, c.status = {upStatus}")
                    .WithParams(new
                    {
                        upName = updatedClass.name,
                        upUrl = updatedClass.url,
                        upPerm = updatedClass.perm,
                        upStatus = updatedClass.status,
                    })
                   .With("c")
                   .Match("(c)<-[rModify:MODIFIED_BY]-(prevClassifier:Classifier)")
                   .Delete("rModify")
                   .With("c")
                   .Match("(recentClassifier:Classifier {email: {modifierEmail} })")
                   .WithParam("modifierEmail", modifier.email)
                   .CreateUnique("(c)<-[rNewModify:MODIFIED_BY]-(recentClassifier)")
                   .Set("rNewModify.lastModified = timestamp()");

                // Update 2) Update the concept string if it's been changed...
                if (updatedClass.conceptStr.ToString() != oldClass.conceptStr.ToString())
                {
                    // We know the ConStr must be updated. The only case where we won't need
                    // to remove any terms is when there were none in the first place.
                    // Similarly for adding terms; we only need to do that if the 
                    // ConStr is not "". Use these bools to make the if/else statements clearer.
                    bool needToRemoveTerms = (oldClass.conceptStr.ToString() != "");
                    bool needToAddTerms = (updatedClass.conceptStr.ToString() != "");

                    buildQuery = buildQuery
                        .With("c")
                        .Match("(c)-[:HAS_CONSTR]->(cs:ConceptString)")
                        .Set("cs.terms = {newConStr}")
                        .WithParam("newConStr", updatedClass.conceptStr.ToString());

                    // If the previous concept string had terms to remove, then remove them.
                    // Otherwise, skip this part
                    if (needToRemoveTerms)
                    {
                        // Find the old terms
                        buildQuery = buildQuery
                                .With("c, cs")
                                .OptionalMatch("(cs)-[rOldTerms:HAS_TERM]->(:Term)")
                                .With("c, cs, COLLECT(rOldTerms) AS toDeleteLater");
                    }
                    else
                    {
                        // Even though we're not removing, we need to have the same column names,
                        // so use  a dummy name holder.
                        buildQuery = buildQuery.With("c, cs, COLLECT(\"\") AS toDeleteLater");
                    }

                    // If we need to add new terms
                    if (needToAddTerms)
                    {
                        buildQuery = buildQuery
                            .With(@"c, cs, toDeleteLater, REPLACE({newConStr}, ""("", """") AS trmStr
                                UNWIND ( FILTER
                                            ( x in
                                                SPLIT( trmStr, "")"" )
                                                WHERE x <> """"
                                            )
                                        ) AS t4")
                            .Match("(matchedT:Term {rawTerm: t4})")
                            .Create("(cs)-[:HAS_TERM]->(matchedT)");

                    }
                    // Finally delete the old terms
                    if (needToRemoveTerms)
                    {
                        buildQuery = buildQuery
                            .With("c, cs, toDeleteLater UNWIND toDeleteLater AS delOldRel")
                            .Delete("delOldRel");
                    }
                }

                var results = buildQuery
                   .With("c.id AS newId")
                   .Return((newId) => new
                   {
                       cId = newId.As<string>(),
                   }).Results.FirstOrDefault();

                if (results != null)
                {
                    return getClassifiableById(results.cId);
                }
            }
            return null;
        }

        /// <summary>
        /// Queries the database to return Classifiables based on a ConceptString.
        /// Use the limit and skip parameters to page the results. 
        /// </summary>
        /// <param name="cstring">A Concepttring to search by.</param>
        /// <param name="optLimit">Default 0. Should be an integer greater than 0.
        /// Indicate how many results to be returned at max.
        /// Set to 0 if you want all the results with at least one match.</param>
        /// <param name="optSkip">Default 0. Should be a positive integer.
        /// Indicates how many results from the top should be
        /// skipped. Set to 0 if no results should be skipped.</param>
        /// <returns>Returns a ClassifiableCollection where each Classifiable's 
        /// ConceptSring has at least one matching term from </returns>
        public ClassifiableCollection getClassifiablesByConStr(ConceptString conStr,
            int optLimit = 0, int optSkip = 0)
        {
            ClassifiableCollection resColl = new ClassifiableCollection
            {
                data = new List<Classifiable>(),
            };

            this.open();

            if (client != null)
            {
                int limit = 0;
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
                    .With("DISTINCT c, cs")
                    .Match("(c)-[:HAS_CONSTR]->(cs)-[:HAS_TERM]->(tActual:Term)")
                    //.With("DISTINCT c, t")
                    .Return((c, tActual) => new
                    {
                        classifiable = c.As<Classifiable>(),
                        terms = tActual.CollectAs<Term>(),
                    })
                    .Skip(skip);

                if (limit > 0)
                {
                    query = query.Limit(limit);
                }

                System.Diagnostics.Debug.WriteLine(query.Query.DebugQueryText);

                var results = query.Results.ToList();

                if (results != null)
                {
                    // Build up Classifiables
                    for (int i = 0; i < results.Count; i++)
                    {
                        var res = results.ElementAt(i);

                        // TODO: reorder terms to match concept string
                        ConceptString resConStr = new ConceptString
                        {
                            terms = new List<Term>(),
                        };

                        foreach (var t in res.terms)
                        {
                            t.Data.subTerms = new List<Term>();
                            resConStr.terms.Add(t.Data);
                        }

                        // A bit of a hack for now. But it maintains order because of
                        // ...some reason.
                        resConStr.terms.Reverse();
                        res.classifiable.conceptStr = resConStr;

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
                        .Create("(b:Term{addMe})-[:SUBTERM_OF]->(a)")
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
        /// Renames a term in the database.
        /// </summary>
        /// <param name="t">The term you want to rename. NB the only matching criteria is ID.</param>
        /// <param name="newRawTerm">The new name for the term.</param>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public bool renameTerm(Term t, String newRawTerm)
        {
            this.open();

            if (client != null)
            {
                var targetSearch = client.Cypher
                    .Match("(a:Term{id:{PARAM1}})")
                    .WithParam("PARAM1", t.id)
                    .Set("a.rawTerm = {PARAM2}, a.lower = {PARAM3}")
                    .WithParam("PARAM2", newRawTerm)
                    .WithParam("PARAM3", newRawTerm.ToLower())
                    .Return(() => Return.As<int>("count(a)"))
                    .Results.DefaultIfEmpty(0).FirstOrDefault();

                if (targetSearch == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
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

        /// <summary>
        /// Checks if the given term can be safely deleted from the database, that is, the given term has no child terms.
        /// </summary>
        /// <param name="t">The term to check. NB the only matching criteria is the term ID.</param>
        /// <returns>True if the term is safe to delete, false otherwise.</returns>
        public bool validateDeleteTerm(Term t)
        {
            this.open();
            if (client != null)
            {
                var numChildren = client
                    .Cypher
                    .Match("(a:Term{id:{PARAM_ID}})<-[:SUBTERM_OF]-(b)")
                    .WithParam("PARAM_ID", t.id)
                    .Return(() => Return.As<int>("count(b)"))
                    .Results.DefaultIfEmpty(0).FirstOrDefault();

                if (numChildren > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                // The client couldn't be opened, so assume we're not ok to delete the term.
                return false;
            }
        }

        /// <summary>
        /// Operates like delTerm, but performs no actual deletion.
        /// </summary>
        /// <param name="t">The term to preview deletion for.</param>
        /// <returns>Nodes that would be affected by the deletion of Term t</returns>
        public AffectedNodes delTermPREVIEW(Term t)
        {
            this.open();

            if (client != null)
            {
                AffectedNodes result = new AffectedNodes();

                List<ConceptString> theStrings = client
                    .Cypher
                    .Match("(a:Term{id:{PARAM_ID}})<-[:HAS_TERM]-(b:ConceptString)")
                    .WithParam("PARAM_ID", t.id)
                    .Return(() => Return.As<ConceptString>("b"))
                    .Results.ToList();

                foreach (ConceptString aString in theStrings)
                {
                    // TODO: doesn't work! :P
                    result.stringsAffected.Add(aString);

                    List<Classifiable> theClassifiables = client
                        .Cypher
                        .Match("(a:ConceptString{terms:{PARAM_STR}})<-[:HAS_CONSTR]-(b:Classifiable)")
                        .WithParam("PARAM_STR", aString.terms)
                        .Return(() => Return.As<Classifiable>("b"))
                        .Results.ToList();

                    foreach (Classifiable aClassifiable in theClassifiables)
                    {
                        result.classifiablesAffected.Add(aClassifiable);
                    }
                }

                return result;
            }
            else
            {
                return new AffectedNodes();
            }
        }

        /// <summary>
        /// Safe-deletes a term from the databse, that is, first checks that the term to be deleted has no child terms.
        /// If the operation cannot be performed because it cannot be validated, this method throws an exception.
        /// If the operation cannot be performed because there was an error in negoriating with the database, the method
        /// returns an new, empty struct.
        /// </summary>
        /// <param name="t">The term to be deleted. NB that ID is the only matching criteria.</param>
        /// <returns>A struct representing the concept strings and classifiables affected by this operation.</returns>
        public AffectedNodes delTerm(Term t)
        {
            if (!validateDeleteTerm(t))
            {
                throw new ArgumentException();
            }

            this.open();

            if (client != null)
            {
                AffectedNodes result = delTermPREVIEW(t);

                delTermFORCE(t);

                return result;
            }
            else
            {
                return new AffectedNodes();
            }
        }

        /// <summary>
        /// Creates a notification for all classifiers. If an email is provided, creates a notification for that user.
        /// </summary>
        /// <param name="message">Notification Message</param>
        /// <param name="classifierEmail">Optional: Create notification for only that user with that email.</param>
        /// <returns></returns>
        public int createNotification(String message, string classifierEmail = "")
        {
            this.open();

            if (client != null)
            {
                var notification = client
                    .Cypher
                    .Create("(a:Notification{msg:{PARAM1},time:TIMESTAMP()})")
                    .WithParam("PARAM1", message)
                    .Return(() => Return.As<Neo4jNotification>("a"))
                    .Results.ToList().ElementAt(0);

                if (notification != null && classifierEmail == "")
                {
                    return client
                        .Cypher
                        .Match("(a:Classifier), (b:Notification{msg:{PARAM1},time:{PARAM2}})")
                        .WithParam("PARAM1", notification.msg)
                        .WithParam("PARAM2", notification.time)
                        .Create("(a)-[r:HAS_NOTIFICATION]->(b)")
                        .Return(() => Return.As<int>("count(r)"))
                        .Results.DefaultIfEmpty(0).FirstOrDefault();
                }
                else if (notification != null && classifierEmail != "")
                {
                    return client
                         .Cypher
                         .Match("(a:Classifier{email:{PARAM0}}), (b:Notification{msg:{PARAM1},time:{PARAM2}})")
                         .WithParam("PARAM0", classifierEmail)
                         .WithParam("PARAM1", notification.msg)
                         .WithParam("PARAM2", notification.time)
                         .Create("(a)-[r:HAS_NOTIFICATION]->(b)")
                         .Return(() => Return.As<int>("count(r)"))
                         .Results.DefaultIfEmpty(0).FirstOrDefault();
                }
                else return 0;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets all notifications for a user.
        /// </summary>
        /// <param name="email">Email of a user.</param>
        /// <returns>Notifications for the user.</returns>
        public List<Neo4jNotification> getNotifications(string email)
        {
            List<Neo4jNotification> rtnNotifications = new List<Neo4jNotification>();

            this.open();

            if (client != null)
            {
                // Query
                // MATCH (n:Notification)<-[:HAS_NOTIFICATION]-(c:Classifier)
                // WHERE c.email = {email}
                // RETURN n.msg AS message, n.time as TIME
                var notificationQ = client.Cypher
                    .Match("(n:Notification)<-[:HAS_NOTIFICATION]-(c:Classifier)")
                    .Where("c.email = {email}").WithParam("email", email)
                    .With("n.msg AS message, n.time AS time")
                    .Return((message, time) => new
                    {
                        nTime = time.As<long>(),
                        nMsg = message.As<string>(),
                    }).Results.ToList();

                if (notificationQ != null)
                {
                    foreach (var note in notificationQ)
                    {
                        Neo4jNotification tmp = new Neo4jNotification
                        {
                            msg = note.nMsg,
                            time = note.nTime,
                        };
                        rtnNotifications.Add(tmp);
                    }
                }
            }
            return rtnNotifications;
        }

        /// <summary>
        /// Returns the number of notifications that have that message and timestamp.
        /// <para>Created for testing purposes.</para>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timestamp"></param>
        /// <returns>Number of notifications with that message and timestamp.</returns>
        public int _notificationExists(Neo4jNotification notification)
        {
            this.open();

            if (client != null)
            {
                return client.Cypher
                        .Match("(n:Notification)")
                        .Where("n.msg = {nMessage}").WithParam("nMessage", notification.msg)
                        .AndWhere("n.time = ToInt({nTime})").WithParam("nTime", notification.time)
                        .Return(() => Return.As<int>("count(*)"))
                        .Results.DefaultIfEmpty(0).FirstOrDefault();
            }
            return 0;
        }
        
        /// <summary>
        /// Removes the provided "Notification".
        /// </summary>
        /// <param name="notification">Notification: String message, String timestamp</param>
        /// <returns>Number of relationships left for that notification.</returns>
        public int removeNotification(String email, Neo4jNotification notification)
        {
            this.open();

            if (client != null)
            {
                // Query
                // MATCH (n:Notification)<-[r:HAS_NOTIFICATION]-(c:Classifier)
                // WHERE c.email = {email} AND n.msg = {nMessage} AND n.time = ToInt({nTime})
                // DELETE r
                // WITH n
                // MATCH (n)<-[remaining:HAS_NOTIFCATION]-()
                // RETURN COUNT(remaining)
                client.Cypher
                    .Match("(n:Notification)<-[r:HAS_NOTIFICATION]-(c:Classifier)")
                    .Where("c.email = {email}").WithParam("email", email)
                    .AndWhere("n.msg = {nMessage}").WithParam("nMessage", notification.msg)
                    .AndWhere("n.time = ToInt({nTime})").WithParam("nTime", notification.time)
                    .Delete("r").ExecuteWithoutResults();
                    
                int removeQ = client.Cypher
                    .Match("(n:Notification)<-[r:HAS_NOTIFICATION]-()")
                    .Where("n.msg = {nMessage}").WithParam("nMessage", notification.msg)
                    .AndWhere("n.time = ToInt({nTime})").WithParam("nTime", notification.time)
                    .Return(() => Return.As<int>("count(*)"))
                    .Results.Single();

                // If there are no more relationships to this notification, get rid of it.
                if (removeQ == 0)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Neo4jDB_num of relationships left: {0:D}", removeQ));
                    _deleteNotification(notification);
                }
            }
            return 0;
        }

        /// <summary>
        /// Actually remove the notification. Call only when there are no relationships left!
        /// </summary>
        /// <param name="message">The notification's message.</param>
        /// <param name="timestamp">The notification's timestamp.</param>
        /// <returns>The number of relationships left. 0 means success!</returns>
        public void _deleteNotification(Neo4jNotification notification)
        {
            this.open();

            if (client != null)
            {
                // Query
                // MATCH (n:Notification)
                // WHERE n.msg = {message} AND n.time = ToInt({time})
                // DELETE n
                client.Cypher
                    .Match("(n:Notification)")
                    .Where("n.msg = {nMessage}").WithParam("nMessage", notification.msg)
                    .AndWhere("n.time = ToInt({nTime})").WithParam("nTime", notification.time)
                    .Delete("n").ExecuteWithoutResults();
            }
        }

        public void cleanupTestMess()
        {
            this.open();

            if (client != null)
            {
                client
                    .Cypher
                    .Match("(a:Term{id:{PARAM}})-[r]-()")
                    .WithParam("PARAM", "TEST_1")
                    .Delete("a, r")
                    .ExecuteWithoutResults();
                client
                    .Cypher
                    .Match("(a:Term{id:{PARAM}})-[r]-()")
                    .WithParam("PARAM", "TEST_2")
                    .Delete("a, r")
                    .ExecuteWithoutResults();
                client
                    .Cypher
                    .Match("(a:Term{id:{PARAM}})-[r]-()")
                    .WithParam("PARAM", "TEST_3")
                    .Delete("a, r")
                    .ExecuteWithoutResults();
                client
                    .Cypher
                    .Match("(a:Term{id:{PARAM}})-[r]-()")
                    .WithParam("PARAM", "CHILD_0")
                    .Delete("a, r")
                    .ExecuteWithoutResults();
                client
                    .Cypher
                    .Match("(a:Term{id:{PARAM}})")
                    .WithParam("PARAM", "CHILD_0")
                    .Delete("a")
                    .ExecuteWithoutResults();
                client
                    .Cypher
                    .Match("(a:Term{id:{PARAM}})-[r]-()")
                    .WithParam("PARAM", "CHILD_1")
                    .Delete("a, r")
                    .ExecuteWithoutResults();
                client
                    .Cypher
                    .Match("(a:Term{id:{PARAM}})")
                    .WithParam("PARAM", "CHILD_1")
                    .Delete("a")
                    .ExecuteWithoutResults();

                // Just some classifiers
                List<string> classEmails01 = new List<string>
                {
                    "newUser@Test.com",
                    "userRepeat@Test.com",
                    "findByEmail@Test.com",
                    "userDeleteMe@Test.com",
                    "testingGetGlamOfMe@BCCNeo4j.com",
                };
                foreach (string email in classEmails01)
                {
                    // Delete all the classifiers
                    client.Cypher
                          .OptionalMatch("(o:Classifier{email: {em} })")
                          .OptionalMatch("(o)-[r:ASSOCIATED_WITH]->(:GLAM)")
                          .WithParam("em", email)
                          .Delete("o,r")
                          .ExecuteWithoutResults();
                }

                // Classifiers with classifiables to remove
                List<string> classEmails02 = new List<string>
                {
                    "testingGetMyClassifiables@BCCNeo4j.com",
                    "testingRecent@BCCNeo4j.com",
                    "testingRecentA@BCCNeo4j.com",
                    "testingRecentB@BCCNeo4j.com",
                    "user2@USNationalParks.com",
                    "testingUnclassedA@BCCNeo4j.com",
                    "testingUnclassedB@BCCNeo4j.com",
                    "testingAddClassi@BCCNeo4j.com",
                    "testingClassiAlreadyExists@BCCNeo4j.com",
                    "testingNoTermsExist@BCCNeo4j.com",
                    "testingNoTerms@BCCNeo4j.com",
                    "testingUpdateSimple@BCCNeo4j.com",
                    "testingUpdateSimpleOwner@BCCNeo4j.com",
                    "testingUpdateSimpleAnother@BCCNeo4j.com",
                    "testingUpdateConStrAdd@BCCNeo4j.com",
                    "testingUpdateConStrEdit@BCCNeo4j.com",
                    "testingUpdateConStrRemove@BCCNeo4j.com",
                    "testingUpdateViolateId@BCCNeo4j.com",
                    "testingUpdatingImproperTerms@BCCNeo4j.com",
                    "testingUnclassifiedMyOwn@testing.com",
                    "testingEditUnclassedOwner@BCCNeo4j.com",
                    "testingEditUnclassedAnother@BCCNeo4j.com",
                    "testingEditRecentOwner@BCCNeo4j.com",
                    "testingEditUnclassedOwner@BCCNeo4j.com",
                    "testingEditUnclassedAnother@BCCNeo4j.com",
                    "testingToDel@BCCNeo4j.com",
                    "testingToDelNoTerms@BCCNeo4j.com",
                    "testingToDel03@BCCNeo4j.com",
                };
                foreach (string email in classEmails02)
                {
                    // Delete all the CLASSIFIABLES of the test users
                    // and any other relationships they might have?
                    client.Cypher
                        .Match("(owner:Classifier {email: {em}})-[r:OWNS]->(c:Classifiable)")
                        .OptionalMatch("(c)-[r1:HAS_CONSTR]->(cs:ConceptString)")
                        .OptionalMatch("(cs)-[r2:HAS_TERM]->(:Term)")
                        .OptionalMatch("(c)-[rOther]-()")
                        .WithParam("em", email)
                        .Delete("r2, r1, cs, r, c, rOther")
                        .ExecuteWithoutResults();

                    // Delete all the classifiers
                    client.Cypher
                          .OptionalMatch("(o:Classifier{email: {em} })")
                          .OptionalMatch("(o)-[r:ASSOCIATED_WITH]->(:GLAM)")
                          .WithParam("em", email)
                          .Delete("o,r")
                          .ExecuteWithoutResults();
                }


                // List of all the classifier emails in testing...
                List<string> classEmails = new List<string>
                {
                    "notifyMeCreate@someplace.com",
                    "notifyMeGetSome@someplace.com",
                    "notifyMeGetNone@someplace.com",
                    "notifyMeRemoveOne@someplace.com",
                    "notifyMeRemoveAll@someplace.com",
                    "notifyMeRemoveMine@someplace.com",
                    "notifyMeKeepMine@someplace.com",
                };

                foreach (string email in classEmails)
                {
                    // DELETE ALL NOTIFICATIONS for the test users
                    client.Cypher
                        .Match("(n:Notification)<-[r:HAS_NOTIFICATION]-(c:Classifier)")
                        .Where("c.email = {em}").WithParam("em", email)
                        .Delete("n, r")
                        .ExecuteWithoutResults();

                    // Delete all the classifiers
                    client.Cypher
                          .OptionalMatch("(o:Classifier{email: {em} })")
                          .OptionalMatch("(o)-[r:ASSOCIATED_WITH]->(:GLAM)")
                          .WithParam("em", email)
                          .Delete("o,r")
                          .ExecuteWithoutResults();
                }

                // Deleting GLAMs
                List<string> glamNames = new List<string>
                {
                    "Test",
                    "Fetched GLAM",
                    "GettingClassifiables01",
                    "Recently Classified",
                    "Recent A vs B",
                    "AddingClassifiableSuccess",
                    "AddClassifiable But Exists",
                    "AddClassifiable ButBadTerms",
                    "AddingWithNoTerms",
                    "Updating GLAM",
                    "Notifications!",
                    "Unclassified My Own",
                    "Recent Uncclassified YoursandOthers",
                    "Recent Unclassified Update Perm",
                    "Recent Unclassified Update Yours",
                    "DeletingAClassifiable",
                };
                foreach (string name in glamNames)
                {
                    client.Cypher
                        .OptionalMatch("(g:GLAM)")
                        .Where("g.name = {name}").WithParam("name", name)
                        .Delete("g")
                        .ExecuteWithoutResults();
                }
            }
        }

        /// <summary>
        /// A data stucture that represents the terms affected by a DELETE operation in the database.
        /// </summary>
        public class AffectedNodes
        {
            public List<ConceptString> stringsAffected
            {
                get;
                set;
            }
            public List<Classifiable> classifiablesAffected
            {
                get;
                set;
            }

            public AffectedNodes()
            {
                stringsAffected = new List<ConceptString>();
                classifiablesAffected = new List<Classifiable>();
            }
        }
    }
}