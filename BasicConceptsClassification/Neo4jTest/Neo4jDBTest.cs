using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Neo4j;
using BCCLib;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;

namespace Neo4jTest
{
    [TestClass]
    public class Neo4jDBTest
    {
        [TestMethod]
        public void Neo4jDB_OpenConnection_Successful()
        {
             var TestConnection = new Neo4jDB();
             TestConnection.open();
             Assert.IsNotNull(TestConnection);
        }

        [TestMethod]
        public void DeleteClassifier_Successful()
        {
            // TODO: ATM it's tested as clean up a cleanup function when adding 
            // a classifiable... 
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void GetClassifiableById_IsClassified_IdExists()
        {
            var conn = new Neo4jDB();

            string searchById = "US National Parks Service_Adze Blade";
            Classifiable classifiedWithGoodId = conn.getClassifiableById(searchById);

            Assert.IsNotNull(classifiedWithGoodId);
            Assert.AreEqual(searchById.ToString(), classifiedWithGoodId.id);
            Assert.AreEqual("Adze Blade", classifiedWithGoodId.name);
            Assert.AreEqual("Classified", Classifiable.Status.Classified.ToString());
            Assert.AreEqual(6, classifiedWithGoodId.conceptStr.terms.Count);
            Assert.AreEqual("(blade)(of)(Tool)(for)(carving)(wood)", classifiedWithGoodId.conceptStr.ToString());
        }

        [TestMethod]
        public void GetClassifiableById_NotClassified_IdExists()
        {
            var conn = new Neo4jDB();

            string searchById = "US National Parks Service_Atlatl Foreshaft";
            Classifiable notClassifiedWithGoodId = conn.getClassifiableById(searchById);

            Assert.IsNotNull(notClassifiedWithGoodId);
            Assert.AreEqual(searchById.ToString(), notClassifiedWithGoodId.id);
            Assert.AreEqual(0, notClassifiedWithGoodId.conceptStr.terms.Count);
            Assert.AreEqual("", notClassifiedWithGoodId.conceptStr.ToString());
        }

        [TestMethod]
        public void GetClassifiableById_DoesNotExist()
        {
            var conn = new Neo4jDB();

            string searchById = "909090";
            Classifiable doesNotExistClassifiable = conn.getClassifiableById(searchById);

            Assert.IsNull(doesNotExistClassifiable);
        }

        [TestMethod]
        public void GetClassiablesByName_ExistsOne() 
        {
            var conn = new Neo4jDB();

            string searchByName = "Adze Blade";
            ClassifiableCollection matchedNameClassifiable = conn.getClassifiablesByName(searchByName);

            Assert.IsNotNull(matchedNameClassifiable.data);
            Assert.AreEqual(1, matchedNameClassifiable.data.Count);
            Assert.AreEqual(searchByName, matchedNameClassifiable.data[0].name);
            Assert.AreEqual(6, matchedNameClassifiable.data[0].conceptStr.terms.Count);
            Assert.AreEqual("(blade)(of)(Tool)(for)(carving)(wood)", matchedNameClassifiable.data[0].conceptStr.ToString());
        }

        [TestMethod]
        public void GetClassifiablesByName_DoesNotExist()
        {
            var conn = new Neo4jDB();

            string searchByName = "909090";
            ClassifiableCollection noMatchedName = conn.getClassifiablesByName(searchByName);

            Assert.IsNotNull(noMatchedName);
            Assert.AreEqual(0, noMatchedName.data.Count);
        }

        [TestMethod]
        public void GetAllUnClassified()
        {
            var conn = new Neo4jDB();

            ClassifiableCollection unclassifieds = conn.getAllUnClassified();

            foreach (var unclassified in unclassifieds.data)
            {
                Assert.AreEqual(0, unclassified.conceptStr.terms.Count);
                Assert.AreEqual("", unclassified.conceptStr.ToString());
            }
        }

        [TestMethod]
        public void AddClassifiable_Suceed()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("Sample", "someurl");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testing@BCCNeo4j.com";

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termTool, 
                }
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName",
                name = "dummyName",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            Classifiable result = conn.addClassifiable(newClassifiable);

            Assert.AreEqual(newClassifiable.id, result.id);
            Assert.AreEqual(newClassifiable.name, result.name);
            Assert.AreEqual(newClassifiable.url, result.url);
            Assert.AreEqual(newClassifiable.perm, result.perm);
            Assert.AreEqual(newClassifiable.status, result.status);
            Assert.AreEqual(newClassifiable.owner.email, result.owner.email);

            Assert.AreEqual(newClassifiable.conceptStr.ToString(),
                result.conceptStr.ToString());
            
            Assert.AreEqual(newClassifiable.conceptStr.terms.Count,
                            result.conceptStr.terms.Count);

            for (int i = 0; i < newClassifiable.conceptStr.terms.Count; i++)
            {
                Assert.AreEqual(newClassifiable.conceptStr.terms[i].ToString(),
                                result.conceptStr.terms[i].ToString());
            }

            conn.deleteClassifiable(result);
            conn.deleteClassifier(classifier);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException),
            "Classifiable information missing or Classifier email was not seted.")]
        public void AddClassifiable_NoClassifier_ThrowException()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("Sample", "someurl");

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termTool, 
                }
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName",
                name = "dummyName",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                conceptStr = conStr,
            };

            Classifiable result = conn.addClassifiable(newClassifiable);
            
        }

        [TestMethod]
        public void AddClassifiable_AlreadyExists()
        {
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void DeleteClassifiable_Suceed()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("Sample", "someurl");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testing@BCCNeo4j.com";

            Term termWood = new Term
            {
                rawTerm = "wood",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termWood, 
                }
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyNameToDelete",
                name = "dummyNameToDelete",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            Classifiable result = conn.addClassifiable(newClassifiable);

            Assert.AreEqual(newClassifiable.id, result.id);

            conn.deleteClassifiable(result);

            Classifiable isGone = conn.getClassifiableById(newClassifiable.id);
            Assert.IsNull(isGone);

            conn.deleteClassifier(classifier);
        }

        [TestMethod]
        public void DeleteClassifiable_DoesNotExist()
        {
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void GetClassifiablesByConceptString_Ordered_ClassifiablesMatch()
        {
            var conn = new Neo4jDB();

            Term termWood = new Term
            {
                rawTerm = "wood",
            };

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            Term termFor = new Term
            {
                rawTerm = "for",
            };

            // good where clause: 
            // WHERE t.rawTerm = "wood" OR t.rawTerm = "for" OR t.rawTerm = "Tool"
            // but put in ids and not rawTerms, since ids are unique....
            // Maybe. Unless we want all kinds of say, "Beautiful"
            ConceptString searchByConStr = new ConceptString
            {
                terms = new List<Term> 
                {
                    termWood, termTool, termFor,
                },
            };
            
            ClassifiableCollection matchedClassifiables = conn.getClassifiablesByConStr(searchByConStr, ordered: true);

            Assert.IsNotNull(matchedClassifiables);

            // Tests that results are in order of most matches in its own concept string
            int prevMatchCount = searchByConStr.terms.Count;
            int currMatchCount = 0;
            Classifiable prevClassifiable = new Classifiable 
            { 
                name = "first",
            };

            for (int i = 0; i < matchedClassifiables.data.Count; i++)
            {
                var classifiable = matchedClassifiables.data[i];

                foreach (Term t in searchByConStr.terms)
                {
                    if (classifiable.conceptStr.ToString().Contains(t.ToString()))
                    {
                        currMatchCount += 1;
                    }
                }

                Assert.AreNotEqual(0, currMatchCount,
                    String.Format("Classifiable has {0} matches at index: {1:D} in results",
                        classifiable.name, i));

                Assert.IsTrue(currMatchCount <= prevMatchCount,
                              String.Format("Index {4}: Current match: {0:D} for {2}, Previous match: {1:D} for {3}",
                                currMatchCount,
                                prevMatchCount,
                                classifiable.name,
                                prevClassifiable.name,
                                i));

                prevMatchCount = currMatchCount;
                prevClassifiable = classifiable;
                currMatchCount = 0;
            }  
        }

        [TestMethod]
        public void GetClassifiablesByConceptString_Unordered_ClassifiablesMatch()
        {
            var conn = new Neo4jDB();

            Term termWood = new Term
            {
                rawTerm = "wood",
            };

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            Term termFor = new Term
            {
                rawTerm = "for",
            };

            // good where clause: 
            // WHERE t.rawTerm = "wood" OR t.rawTerm = "for" OR t.rawTerm = "Tool"
            // but put in ids and not rawTerms, since ids are unique....
            // Maybe. Unless we want all kinds of say, "Beautiful"
            ConceptString searchByConStr = new ConceptString
            {
                terms = new List<Term> 
                {
                    termWood, termTool, termFor,
                },
            };

            
            ClassifiableCollection matchedClassifiables = conn.getClassifiablesByConStr(searchByConStr);

            Assert.IsNotNull(matchedClassifiables);

            // Only tests that each Classifiable's ConceptString does contain at least one of the
            // terms that was searched by.
            foreach (var classifiable in matchedClassifiables.data)
            {
                bool test = classifiable.conceptStr.ToString().Contains(termWood.rawTerm) ||
                            classifiable.conceptStr.ToString().Contains(termTool.rawTerm) ||
                            classifiable.conceptStr.ToString().Contains(termFor.rawTerm);
                Assert.IsTrue(test);
            }
             
        }

        [TestMethod]
        public void GetTermByRaw_Exists()
        {
            var conn = new Neo4jDB();

            string searchByRawTerm = "Art";
            Term artTerm = conn.getTermByRaw(searchByRawTerm);

            Assert.AreEqual(searchByRawTerm, artTerm.rawTerm);
            Assert.AreEqual("art", artTerm.lower);
            Assert.IsNotNull(artTerm.id);
            Assert.IsNotNull(artTerm.subTerms);
        }

        [TestMethod]
        public void GetTermByLower_Exists()
        {
            var conn = new Neo4jDB();

            string searchByLowerTerm = "art";
            Term term1 = conn.getTermByLower(searchByLowerTerm);

            Assert.AreEqual(searchByLowerTerm, term1.lower);
            Assert.AreEqual("Art", term1.rawTerm);
            Assert.IsNotNull(term1.id);
            Assert.IsNotNull(term1.subTerms);
        }

        [TestMethod]
        public void GetChildrenOfTerm_RootTerm_Exists()
        {
            var conn = new Neo4jDB();

            string rootRaw = "Top Object";
            int NOT_FOUND = -1;

            // These are the rawTerms that are one depth away from the root.
            List<string> rootSubTerms = new List<string>
            {
                "Art", 
                "Biological Entities", 
                "Celestial Objects",
                "Changes within a thing", 
                "Culture", 
                "Economy",
                "Flora and Fauna", 
                "Genetic Predisposition", 
                "Health and population", 
                "Individual differences",
                "Mathematical Concepts", 
                "Molecules and Elements",
                "Non-human Environment", 
                "Politics",
                "Properties, Qualities", 
                "Rocks", 
                "Social Structure",
                "Technology and Science", 
                "Waves and Particles",
            };

            Term rootTerm = conn.getTermByRaw(rootRaw);
            Assert.IsNotNull(rootTerm);

            rootTerm.subTerms = conn.getChildrenOfTerm(rootTerm);

            // Make sure they're the same length
            Assert.AreEqual(rootSubTerms.Count, rootTerm.subTerms.Count);

            foreach (string expectedSubTermStr in rootSubTerms)
            {
                Term expectedSubTerm = new Term
                {
                    rawTerm = expectedSubTermStr,
                };
                Assert.AreNotEqual(NOT_FOUND, rootTerm.hasSubTerm(expectedSubTerm));
            }
        }

        [TestMethod]
        public void GetChildrenOfTerm_NoChildren()
        {
            var conn = new Neo4jDB();
            string searchRawTerm = "Biology";

            Term biologyTerm = conn.getTermByRaw(searchRawTerm);
            Assert.IsNotNull(biologyTerm);

            biologyTerm.subTerms = conn.getChildrenOfTerm(biologyTerm);
            Assert.AreEqual(0,biologyTerm.subTerms.Count);
        }

        [TestMethod]
        public void SearchForNonTerm_ManyFunctions_ReturnNull()
        {
            var conn = new Neo4jDB();

            Term NotExistTerm = new Term
            {
                rawTerm = "I am NOT a Term",
                subTerms = new List<Term>(),
            };

            Term resTerm = conn.getTermByRaw(NotExistTerm.rawTerm);
            Assert.IsNull(resTerm);

            List<Term> resTerm2 = conn.getChildrenOfTerm(NotExistTerm);
            Assert.AreEqual(0, resTerm2.Count);

            Term resTerm3 = conn.getBccFromTermWithDepth(NotExistTerm, 0);
            Assert.IsNull(resTerm3);

            Term resTerm4 = conn.getBccFromTermWithDepth(NotExistTerm, 2);
            Assert.IsNull(resTerm4);

            Term resTerm5 = conn.getBccFromTerm(NotExistTerm);
            Assert.IsNull(resTerm5);
        }

        [TestMethod]
        public void GetBccFromTermWithDepth_NaturalSciencesMaxDepth_Exists()
        {
            var conn = new Neo4jDB();

            int MAX_DEPTH = -1;
            int NOT_FOUND = -1;

            string searchRawTerm = "Natural Sciences";
            List<string> expectedNaturalSciencesSubTermStr = new List<string>
            {
                "Biology",
                "Chemistry",
                "Computer science",
                "Geology",
                "Physics",
            };

            Term naturalSciencesTerm = conn.getTermByRaw(searchRawTerm);

            Term naturalSciencesTree = conn.getBccFromTermWithDepth(naturalSciencesTerm, 
                                                                    MAX_DEPTH);

            Assert.AreEqual(searchRawTerm, naturalSciencesTree.rawTerm);
            Assert.AreEqual(expectedNaturalSciencesSubTermStr.Count, 
                            naturalSciencesTree.subTerms.Count);

            // Expected results, not in a particular order.
            // Natural Sciences - Biology, 
            //                  - Chemistry
            //                  - Physics
            //                      - Astronomy
            //                  - Computer Science
            //                  - Geology
            foreach (string rawSubTermStr in expectedNaturalSciencesSubTermStr)
            {
                Term expectedSubTerm = new Term 
                {
                    rawTerm = rawSubTermStr,
                };

                int index = naturalSciencesTree.hasSubTerm(expectedSubTerm);
                Assert.AreNotEqual(NOT_FOUND, index);

                // Physics has Astronmy underneath it.
                if (searchRawTerm == "Physics") 
                {
                    Term astronomy = new Term
                    {
                        rawTerm = "Astronomy"
                    };
                    Assert.AreNotEqual(NOT_FOUND, naturalSciencesTree.subTerms[index].hasSubTerm(astronomy));
                }
            }
        }

        [TestMethod]
        public void GetBccFromTermWithDepth_WavesAndParticlesDepth2_OnlyHasDepth2()
        {
            var conn = new Neo4jDB();

            int depth = 2;
            int EXPECTED_NUM_SUBTERMS = 6;

            string searchRawTerm = "Waves and Particles";

            Term wavesParticlesTerm = conn.getTermByRaw(searchRawTerm);

            Term wavesParticlesRootD2 = conn.getBccFromTermWithDepth(wavesParticlesTerm, depth);

            // "Waves and Particles" has subTerms, and each of those subTerms
            // also has subTerms. Makes it a bit easier to test that we only get 
            // a depth of 2 for results when the subTerm results are (currently) 
            // not organized alphabetically. 
            Assert.AreEqual(searchRawTerm, wavesParticlesRootD2.rawTerm);
            Assert.AreEqual(EXPECTED_NUM_SUBTERMS, wavesParticlesRootD2.subTerms.Count);
            Assert.AreNotEqual(0, wavesParticlesRootD2.subTerms[0].subTerms.Count);
            Assert.AreEqual(0, wavesParticlesRootD2.subTerms[0].subTerms[0].subTerms.Count);
        }
    
        [TestMethod]
        public void GetRootTerm_BccRoot_Exists()
        {
            var conn = new Neo4jDB();

            Term bccRootTerm = conn.getRootTerm();

            Assert.IsNotNull(bccRootTerm);
            Assert.AreEqual("bccRoot", bccRootTerm.id);
            Assert.AreEqual("bccroot", bccRootTerm.lower);
            Assert.AreEqual("BccRoot", bccRootTerm.rawTerm);
            Assert.IsNotNull(bccRootTerm.subTerms);
            Assert.AreEqual(0, bccRootTerm.subTerms.Count);
        }

        [TestMethod]
        public void GetBccFromRootWithDepth_BccRootD1_Exists()
        {
            int depth1 = 1;

            Term rootTermWithRawTerm = new Term
            {
                rawTerm = "ROOT TERM",
            };

            Term topObjPropertyWithRawTerm = new Term
            {
                rawTerm = "TOP OBJECT PROPERTY",
            };

            Term notRealTermWithRawTerm = new Term
            {
                rawTerm = "Not a Real Term",
            };

            var conn = new Neo4jDB();

            Term bccRootTermD1 = conn.getBccFromRootWithDepth(depth1);

            Assert.IsNotNull(bccRootTermD1);
            Assert.AreEqual("bccroot", bccRootTermD1.lower);
            Assert.IsNotNull(bccRootTermD1.subTerms.Count);
            Assert.IsNotNull(bccRootTermD1.hasSubTerm(rootTermWithRawTerm));
            Assert.IsNotNull(bccRootTermD1.hasSubTerm(topObjPropertyWithRawTerm));
            Assert.IsNotNull(bccRootTermD1.hasSubTerm(notRealTermWithRawTerm));
        }
    }
}