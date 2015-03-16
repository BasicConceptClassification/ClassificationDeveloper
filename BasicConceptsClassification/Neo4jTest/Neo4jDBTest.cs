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
        public void AddClassiier_Successful()
        {
            GLAM glam = new GLAM("US National Parks Service");

            Classifier classifier = new Classifier(glam);
            classifier.email = "user99@USNationalParks.com";
            
            var conn = new Neo4jDB();

            Classifier addedClassifier = conn.addClassifier(classifier);

            Assert.IsNotNull(addedClassifier);
            Assert.AreEqual(classifier.email, addedClassifier.email);
            Assert.AreEqual(glam.name, addedClassifier.getOrganizationName());

            conn.deleteClassifier(addedClassifier);
        }

        [TestMethod]
        public void AddClassifier_AlreadyExists_ThrowException()
        {
            GLAM glam = new GLAM("US National Parks Service");

            Classifier classifier = new Classifier(glam);
            classifier.email = "user99@USNationalParks.com";

            var conn = new Neo4jDB();

            Classifier addedClassifier = conn.addClassifier(classifier);
           
            // TODO: once we have test setup and cleanup, use those instead
            // of try-catch to do the clean up...
            try
            {
                Classifier addedClassifier2 = conn.addClassifier(addedClassifier);
            }
            catch (Exception)
            {
            }
            conn.deleteClassifier(addedClassifier);
            
        }

        [TestMethod]
        public void GetClassifier_ByEmail_Exists()
        {
            string classifierEmail = "user1@USNationalParks.com";

            var conn = new Neo4jDB();

            Classifier foundClassifier = conn.getClassifier(classifierEmail);

            Assert.IsNotNull(foundClassifier);
            Assert.AreEqual(classifierEmail, foundClassifier.email);
             
        }

        [TestMethod]
        public void GetClassifier_ByEmail_DoesNotExist()
        {
            string classifierEmail = "userDoesNotExist@USNationalParks.com";

            var conn = new Neo4jDB();

            Classifier foundClassifier = conn.getClassifier(classifierEmail);

            Assert.IsNull(foundClassifier);
        }

        [TestMethod]
        public void DeleteClassifier_Successful()
        {
            GLAM glam = new GLAM("US National Parks Service");
            Classifier classifier = new Classifier(glam);
            classifier.email = "userDeleteMe@USNationalParks.com";

            var conn = new Neo4jDB();

            Classifier addedClassifier = conn.addClassifier(classifier);

            conn.deleteClassifier(addedClassifier);

            // Make sure the classifier no longer exists
            Classifier notFound = conn.getClassifier(addedClassifier.email);
            Assert.IsNull(notFound);
        }

        [TestMethod]
        public void GetGlamOfClassifier_Success()
        {
            //getGlamOfClassifier("someemail@somewhere.com");
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void GetAllGlams_Exists()
        {
            var conn = new Neo4jDB();

            List<GLAM> resGlam = conn.getAllGlams();

            // TODO: sample data should have a GLAM called AAAAAAAA
            // and therefore it would be the first one alphabetically.
            Assert.AreNotEqual(0, resGlam.Count);
            Assert.AreEqual("Achaemenid GLAM", resGlam[0].name);
        }

        [TestMethod]
        public void GetClassifiableById_IsClassified_IdExists()
        {
            var conn = new Neo4jDB();

            String searchById = "US National Parks Service_Adze Blade";
            Classifiable classifiedWithGoodId = conn.getClassifiableById(searchById);

            Assert.IsNotNull(classifiedWithGoodId);
            Assert.AreEqual(searchById.ToString(), classifiedWithGoodId.id);
            Assert.AreEqual("Adze Blade", classifiedWithGoodId.name);
            Assert.AreEqual(6, classifiedWithGoodId.conceptStr.terms.Count);
            Assert.AreEqual("(blade)(of)(Tool)(for)(carving)(wood)", classifiedWithGoodId.conceptStr.ToString());
        }

        [TestMethod]
        public void GetClassifiableById_NotClassified_IdExists()
        {
            var conn = new Neo4jDB();

            String searchById = "US National Parks Service_Atlatl Foreshaft";
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

            String searchById = "909090";
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
        public void GetRecentlyClassified_Yours_AfterTwoAdds()
        {
            // Get your recently test steps:
            // 1) Classify something 
            // 2) Get recently classified
            // 3) Classify again
            // 4) Get recently classified and should be in order...
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("Sample");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingRecent@BCCNeo4j.com";

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
                id = glam.name + "_" + "dummyAdd01",
                name = "dummyAdd01",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            conn.addClassifier(classifier);

            // Add the first classifiable and get the recent results
            Classifiable result = conn.addClassifiable(newClassifiable);
            ClassifiableCollection recent1 = conn.getRecentlyClassified(classifier.email);

            // Test to make sure we got one result for that new classifier
            Assert.AreEqual(1, recent1.data.Count);
            Assert.AreEqual(newClassifiable.name, recent1.data[0].name);

            // Create the second classifiable
            Classifiable newClassifiable2 = newClassifiable;
            newClassifiable2.name = "dummyAdd02";
            newClassifiable2.id = glam.name + "_" + newClassifiable2.name;

            // Add the second classifiable
            Classifiable result2 = conn.addClassifiable(newClassifiable2);
            ClassifiableCollection recent2 = conn.getRecentlyClassified(classifier.email);

            // Test to make sure we get two classifiables and the one we just added is first
            Assert.AreEqual(2, recent2.data.Count);
            Assert.AreEqual(result2.name, recent2.data[0].name);
            Assert.AreEqual(result.name, recent2.data[1].name);

            // Clean up
            conn.deleteClassifiable(result);
            conn.deleteClassifiable(result2);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void GetRecentlyClassified_YoursOnly()
        {
            // Steps:
            // 1) ClassifierA adds Classifiable A_1, perm GLAM
            //      Recent A: A_1
            // 2) ClassifierA adds Classifiable A_2, perm OwnerOnly
            //      Recent A: A_2, A_1
            // 3) Classifier B adds Classifiable B_1, perm GLAM
            //      Recent A: B_1, A_2, A_1
            // 4) Classifier A adds Classifiable A_3, perm GLAM
            //      Recent A: A_3, A_2, A_1
            //      Recent B: B_1
            GLAM glam = new GLAM("Recent A vs B");

            Classifier classifierA = new Classifier(glam);
            classifierA.email = "testingRecentA@BCCNeo4j.com";

            Classifier classifierB = new Classifier(glam);
            classifierB.email = "testingRecentB@BCCNeo4j.com";

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

            Classifiable A1GLAM = new Classifiable
            {
                id = glam.name + "_" + "A1 GLAM",
                name = "A1 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            Classifiable A2OwnerOnly = new Classifiable
            {
                id = glam.name + "_" + "A2 OwnerOnly",
                name = "A2 OwnerOnly",
                url = "dummyURL",
                perm = Classifiable.Persmission.OwnerOnly.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            Classifiable A3GLAM = new Classifiable
            {
                id = glam.name + "_" + "A3 GLAM",
                name = "A3 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            Classifiable B1GLAM = new Classifiable
            {
                id = glam.name + "_" + "B1 GLAM",
                name = "B1 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifierB,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();

            conn.addClassifier(classifierA);
            conn.addClassifier(classifierB);

            // Add the Classifiables in this order
            Classifiable resA1 = conn.addClassifiable(A1GLAM);
            Classifiable resA2 = conn.addClassifiable(A2OwnerOnly);
            Classifiable resB1 = conn.addClassifiable(B1GLAM);
            Classifiable resA3 = conn.addClassifiable(A3GLAM);

            ClassifiableCollection recentA = conn.getRecentlyClassified(classifierA.email);
            ClassifiableCollection recentB = conn.getRecentlyClassified(classifierB.email);

            // Verify Classifier A's recently modified
            Assert.AreEqual(3, recentA.data.Count);
            Assert.AreEqual(A3GLAM.name, recentA.data[0].name);
            Assert.AreEqual(A2OwnerOnly.name, recentA.data[1].name);
            Assert.AreEqual(A1GLAM.name, recentA.data[2].name);

            // Verify Classifier B's recently modified
            Assert.AreEqual(1, recentB.data.Count);
            Assert.AreEqual(B1GLAM.name, recentB.data[0].name);

            conn.deleteClassifiable(A1GLAM);
            conn.deleteClassifiable(A2OwnerOnly);
            conn.deleteClassifiable(A3GLAM);
            conn.deleteClassifiable(B1GLAM);
            conn.deleteClassifier(classifierA);
            conn.deleteClassifier(classifierB);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void GetAllUnclassified_YourOwn_Exists()
        {
            GLAM glam = new GLAM("US National Parks Service");
            Classifier classifier = new Classifier(glam);
            classifier.email = "user1@USNationalParks.com";

            var conn = new Neo4jDB();

            ClassifiableCollection unclassifieds = conn.getAllUnclassified(classifier.email);

            // TODO: fix: Bad test without sample data, but will do for now
            Assert.AreNotEqual(0, unclassifieds.data.Count);

            // check cases:
            // not all may have a null constr
            // some may not have a ConceptString
            foreach (var unclassified in unclassifieds.data)
            {
                Assert.AreEqual(0, unclassified.conceptStr.terms.Count);
                Assert.AreEqual("", unclassified.conceptStr.ToString());
            }
        }

        [TestMethod]
        public void GetAllUnclassified_NotYours_Exists()
        {
            GLAM glam = new GLAM("US National Parks Service");
            Classifier classifier = new Classifier(glam);
            classifier.email = "user2@USNationalParks.com";

            var conn = new Neo4jDB();

            conn.addClassifier(classifier);

            ClassifiableCollection unclassifieds = conn.getAllUnclassified(classifier.email);

            // TODO: fix: Bad test without sample data, but will do for now
            Assert.AreNotEqual(0, unclassifieds.data.Count);

            // check cases:
            // not all may have a null constr
            // some may not have a ConceptString
            // Since we just added the classifier and didn't add any classifiables to them,
            // We can check that all the Classifiables they should have should not
            // have the permission OwnerOnly.
            foreach (var unclassified in unclassifieds.data)
            {
                Assert.AreNotEqual(Classifiable.Persmission.OwnerOnly,
                    unclassified.perm);
                Assert.AreEqual(0, unclassified.conceptStr.terms.Count);
                Assert.AreEqual("", unclassified.conceptStr.ToString());
            }

            conn.deleteClassifier(classifier);
        }

        [TestMethod]
        public void GettAllUnclassified_YoursAndOthers()
        {
            Assert.IsTrue(false);

            // Steps:
            // 1) ClassifierA adds Classifiable A_1, perm GLAM
            //      Recent A: A_1
            // 2) ClassifierA adds Classifiable A_2, perm OwnerOnly
            //      Recent A: A_2, A_1
            // 3) Classifier B adds Classifiable B_1, perm GLAM
            //      Recent A: B_1, A_2, A_1
            // 4) Classifier A adds Classifiable A_3, perm GLAM
            //      Recent A: A_3, B_1, A_2, A_1
            //      Recent B: A_3, B_1, A_1
            GLAM glam = new GLAM("Recent A vs B");

            Classifier classifierA = new Classifier(glam);
            classifierA.email = "testingRecentA@BCCNeo4j.com";

            Classifier classifierB = new Classifier(glam);
            classifierB.email = "testingRecentB@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

            Classifiable A1GLAM = new Classifiable
            {
                id = glam.name + "_" + "A1 GLAM",
                name = "A1 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            Classifiable A2OwnerOnly = new Classifiable
            {
                id = glam.name + "_" + "A2 OwnerOnly",
                name = "A2 OwnerOnly",
                url = "dummyURL",
                perm = Classifiable.Persmission.OwnerOnly.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            Classifiable A3GLAM = new Classifiable
            {
                id = glam.name + "_" + "A3 GLAM",
                name = "A3 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            Classifiable B1GLAM = new Classifiable
            {
                id = glam.name + "_" + "B1 GLAM",
                name = "B1 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifierB,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();

            // Add the two classifiers (since we need the glams)
            conn.addClassifier(classifierA);
            conn.addClassifier(classifierB);

            // Add the Classifiables in this order
            Classifiable resA1 = conn.addClassifiable(A1GLAM);
            Classifiable resA2 = conn.addClassifiable(A2OwnerOnly);
            Classifiable resB1 = conn.addClassifiable(B1GLAM);
            Classifiable resA3 = conn.addClassifiable(A3GLAM);

            ClassifiableCollection recentA = conn.getRecentlyClassified(classifierA.email);
            ClassifiableCollection recentB = conn.getRecentlyClassified(classifierB.email);

            Assert.AreEqual(4, recentA.data.Count);
            Assert.AreEqual(3, recentB.data.Count);

            // Recent A: A_3, B_1, A_2, A_1
            Assert.AreEqual(A3GLAM.name, recentA.data[0].name);
            Assert.AreEqual(B1GLAM.name, recentA.data[1].name);
            Assert.AreEqual(A2OwnerOnly.name, recentA.data[2].name);
            Assert.AreEqual(A1GLAM.name, recentA.data[3].name);

            // Recent B: A_3, B_1, A_1

            // Clean up
            conn.deleteClassifiable(resA1);
            conn.deleteClassifiable(resA2);
            conn.deleteClassifiable(resA3);
            conn.deleteClassifiable(resB1);
            conn.deleteClassifier(classifierA);
            conn.deleteClassifier(classifierB);
            conn.deleteGlam(glam);

        }

        [TestMethod]
        public void GetAllUnclassified_OtherTypes()
        {
            // We have new status types to worry about; pending, need help(?),
            // etc. ATM the function only gets those with status of "Unclassified".
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void GettAllUnclassified_HasNone()
        {
            GLAM glam = new GLAM("NonExisting");
            Classifier classifier = new Classifier(glam);
            classifier.email = "userDoNotExist@USNationalParks.com";

            var conn = new Neo4jDB();

            ClassifiableCollection unclassifieds = conn.getAllUnclassified(classifier.email);

            Assert.AreEqual(0, unclassifieds.data.Count);
        }

        [TestMethod]
        public void AddClassifiable_Succeed()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("Sample");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testing1@BCCNeo4j.com";

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
                id = glam.name + "_" + "dummyName1",
                name = "dummyName1",
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
            conn.deleteGlam(glam);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException),
            "Classifiable information missing or Classifier email was not seted.")]
        public void AddClassifiable_NoClassifier_ThrowNullReferenceException()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("Sample");

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
                id = glam.name + "_" + "dummyName2",
                name = "dummyName2",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                conceptStr = conStr,
            };

            Classifiable result = conn.addClassifiable(newClassifiable);
        }

        [TestMethod]
        public void AddClassifiable_AlreadyExists_ThrowNeoException()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("Sample");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testing3@BCCNeo4j.com";

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
                id = glam.name + "_" + "dummyName3",
                name = "dummyName3",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            Classifiable result = conn.addClassifiable(newClassifiable);
            result.url = "anotherDummyUrl";

            // Try adding another Classifiable, but with the same id. Should
            // throw an exception.
            // TODO: once we have setup and clean up methods, remove this try-catch
            // so that the clean doesn't have to be handled in this test function
            try
            {
                Classifiable result2 = conn.addClassifiable(result);
            }
            catch (Exception)
            {
            }
            conn.deleteClassifiable(result);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception),
            "Some Terms are not in the Classification!")]
        public void AddClassifiable_TermsDoNotExist()
        {

            var conn = new Neo4jDB();

            GLAM glam = new GLAM("Sample");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testing4@BCCNeo4j.com";

            Term termTooool = new Term
            {
                rawTerm = "Tooool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termTooool, 
                }
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName4",
                name = "dummyName4",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            Classifiable result = conn.addClassifiable(newClassifiable);

            conn.deleteClassifiable(result);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void AddClassifiable_WithNoTerms()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("Sample");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testing5@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName5",
                name = "dummyName5",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            Classifiable result = conn.addClassifiable(newClassifiable);

            conn.deleteClassifiable(result);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void UpdateClassifiable_SimpleNoConStr_Success()
        {
            // Might merge with the one below (updating all properties)
            // but as a start have them split up
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void UpdateClassifiable_AllProperties_Success()
        {
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void UpdateClassifiable_UniqueIdViolation_ThrowException()
        {
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void DeleteClassifiable_Suceed()
        {
            GLAM glam = new GLAM("Sample");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingToDel@BCCNeo4j.com";

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

            var conn = new Neo4jDB();
            Classifiable result = conn.addClassifiable(newClassifiable);

            conn.deleteClassifiable(result);

            Classifiable isGone = conn.getClassifiableById(newClassifiable.id);
            Assert.IsNull(isGone);

            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void DeleteClassifiable_NoTerms_Succeed()
        {
            GLAM glam = new GLAM("Sample");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingToDel02@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyNameToDelete02",
                name = "dummyNameToDelete02",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();
            Classifiable result = conn.addClassifiable(newClassifiable);

            conn.deleteClassifiable(result);

            Classifiable isGone = conn.getClassifiableById(newClassifiable.id);
            Assert.IsNull(isGone);

            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void DeleteClassifiable_DoesNotExist()
        {
            GLAM glam = new GLAM("Sample");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingToDel03@BCCNeo4j.com";

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

            Classifiable classifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyNameToDelete03",
                name = "dummyNameToDelete03",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();

            conn.deleteClassifiable(classifiable);
            conn.deleteGlam(glam);
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
            Assert.AreEqual(0, biologyTerm.subTerms.Count);
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

        [TestMethod]
        public void TestAddTerm()
        {
            var conn = new Neo4jDB();

            Term t1 = new Term
            {
                id = "TEST_1",
                rawTerm = "Test 1",
                lower = "test 1",
                subTerms = new List<Term>()
            };

            for (int i = 0; i < 3; i++)
            {
                Term subterm = new Term
                {
                    id = "CHILD_" + i,
                    rawTerm = "Child " + i,
                    lower = "child " + i
                };

                t1.subTerms.Add(subterm);
            }

            Assert.AreEqual<int>(4, conn.addTerm(t1, null));

            // Cleanup the db changes we made.
            foreach (Term subterm in t1.subTerms)
            {
                Assert.AreEqual<int>(2, conn.delTermFORCE(subterm));
            }
            Assert.AreEqual<int>(2, conn.delTermFORCE(t1));
        }

        [TestMethod]
        public void TestMoveTerm()
        {
            var conn = new Neo4jDB();

            // Create 2 terms.
            Term t1 = new Term
            {
                id = "TEST_1",
                rawTerm = "Test 1",
                lower = "test 1"
            };
            Term t2 = new Term
            {
                id = "TEST_2",
                rawTerm = "Test 2",
                lower = "test 2"
            };

            // Add both to the root node.
            Assert.AreEqual<int>(1, conn.addTerm(t1, null));
            Assert.AreEqual<int>(1, conn.addTerm(t2, null));

            // Move t1 to be a subterm of t2
            Assert.AreEqual<int>(2, conn.moveTerm(t1, t2));

            // Clean up the mess
            Assert.AreEqual<int>(2, conn.delTermFORCE(t1));
            Assert.AreEqual<int>(2, conn.delTermFORCE(t2));
        }
    }
}