using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCCLib;
using Neo4j;
using System.Web.Security;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BCCApplication.Account
{
    public partial class ClassOb : System.Web.UI.Page
    {
        // How much the tree should be expanded by when it needs to expand.
        static int EXPAND_DEPTH = 2;

        private string DESCRIPTION = @"<p>Fill in the required fields below about your GLAM object. The name of the GLAM object and
                                        the URL where a user can find more information about the object are required. 
                                        The name of the GLAM object must be unique to your GLAM. </p>
                                        <p>You can set the permission of the GLAM object to allow only you to edit it or you can set it
                                        so that other classifiers in your GLAM can edit it. 
                                        When adding the GLAM object's Concept String, you may only use the Terms provided. 
                                        If you leave the Concept String blank it will be considered Not Classified. </p>
                                        <p>You can edit any of these later.</p>";


        private string SUCCESS_ADD = "Successfully added: ";
        private string FAIL_UNIQUE = "Failed: Another GLAM Object with that name already exists in your GLAM.";
        private string FAIL_TERMS = "Not all the terms in the concept string are from the controlled vocabulary.";
        protected const string ERROR_SERVER = "Sorry, error with the server!";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LabelDescription.Text = DESCRIPTION;
                GenerateInitialBCCTree();
            }  
        }

        
        protected void GenerateInitialBCCTree()
        {
            // If server is down, display an error message
            try
            {
                // Fetch BCC from the DB
                var dbConn = new Neo4jDB();
                Term bccRootTerm = dbConn.getBccFromRootWithDepth(EXPAND_DEPTH);

                DataSet.Nodes.Clear();
                // Create a starting TreeNode as the root to generate the BCC
                TreeNode currentNode = new TreeNode();
                DataSet.Nodes.Add(generateBccTree(bccRootTerm, currentNode));

                // By default, leave collapsed
                DataSet.CollapseAll();
                DataSet.ShowCheckBoxes = TreeNodeTypes.Leaf;
                LabelNoticationDataSet.Text = "";
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine(Ex.Message);
                LabelNoticationDataSet.Text = ERROR_SERVER;
            }
        }


        /// <summary>
        /// Converts a Term and its subTerms/children Terms to a TreeNode. 
        /// </summary>
        /// <param name="currentTerm">Current Term in the recursion.</param>
        /// <param name="currentNode">Current TreeNode in the recursion.</param>
        /// <returns>TreeNode reprentation of a Term.</returns>
        protected TreeNode generateBccTree(Term currentTerm, TreeNode currentNode)
        {
            String nodeTerm = currentTerm.rawTerm;

            // Create/set the current node we're on: the text displayed with the
            // node and the action when selected.
            currentNode = new TreeNode(nodeTerm);
            currentNode.SelectAction = TreeNodeSelectAction.None;

            // Sort subTerms by alphabetical order
            currentTerm.sortSubTerms();

            if (currentTerm.subTerms.Count > 0)
            {
                // Foreach child, recursively build this up
                foreach (var childTerm in currentTerm.subTerms)
                {
                    currentNode.ChildNodes.Add(generateBccTree(childTerm, currentNode));
                }
            }
            else
            {
                // If there are no children, then there's a chance to populate!
                currentNode.PopulateOnDemand = true;
            }
            return currentNode;
        }

        /// <summary>
        /// Populates the DataSet BCC Tree OnDemand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PopulateNode(Object sender, TreeNodeEventArgs e)
        {
            TreeNode currentNode = e.Node;

            Term reference = new Term
            {
                rawTerm = currentNode.Text,
            };

            var dbConn = new Neo4jDB();
            try
            {
                Term bccRootTerm = dbConn.getBccFromTermWithDepth(reference, EXPAND_DEPTH);
                foreach (var term in bccRootTerm.subTerms)
                {
                    // Create a starting TreeNode as the root to generate the BCC
                    currentNode.ChildNodes.Add(generateBccTree(term, new TreeNode()));
                }
                LabelNoticationDataSet.Text = "";
            }
            catch
            {
                // This doesn't show up... but it's here in case it might?
                LabelNoticationDataSet.Text = ERROR_SERVER;
            }
        }

        //the control submit button
        protected void SubmitObj_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ClassOb_BUTTON-SUBMIT: Click registered");
            //input of each text box
            //get vaule of each
            string inputUrl = ObURL.Text;
            string inputName = ObName.Text;
            string inputConcept = ObConcept.Text;

            // Get the logged in user's email
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;

            // Get user's information for adding the new classifiable
            var conn = new Neo4jDB();
            GLAM gl = new GLAM("");

            try
            {
                gl = conn.getGlamOfClassifier(userEmail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return;
            }

            string username = Context.GetOwinContext().Authentication.User.Identity.Name;
            string email = userEmail;
            Classifier classifier = new Classifier(gl, email, username);

            // Extract the terms from the concept string
            string trimConceptString = inputConcept.Trim();
            List<Term> newTerms = new List<Term>();
            
            // Only extract terms if there are any terms to extract
            if (trimConceptString != "")
            {
                string sstring = trimConceptString.Replace(")(", ",");
                sstring = sstring.Replace(")", "");
                sstring = sstring.Replace("(", "");
                //new_str is the result list 
                List<string> newStr = sstring.Split(',').ToList();

                foreach (String termStr in newStr)
                {
                    //change to terms
                    Term terterma = new Term { rawTerm = termStr, };
                    newTerms.Add(terterma);
                }
            }

            ConceptString newConceptStr = new ConceptString
            {
                terms = newTerms,
            };

            // Create the new Classifiable's information.
            Classifiable newClassifiable = new Classifiable
            {
                id = classifier.getOrganizationName() + "_" + inputName,
                name = inputName,
                url = inputUrl,
                perm = EditPerm.SelectedValue,
                owner = classifier,
                conceptStr = newConceptStr,
            };

            // Simple statuses for now. If there is terms in the concept string, then
            // it's considered to be Classified. If there are none, then it's Unclassified.
            if (newClassifiable.conceptStr.ToString() == "")
            {
                newClassifiable.status = Classifiable.Status.Unclassified.ToString();
            }
            else
            {
                newClassifiable.status = Classifiable.Status.Classified.ToString();
            }

            // Try to add the newClassifiable and display an error message depending on the result.
            try
            {
                Classifiable result = conn.addClassifiable(newClassifiable);

                if (result != null)
                {
                    ObAddStatus.Text = String.Format("{0} {1}.", SUCCESS_ADD, result.name);
                }

            }
            catch (ArgumentException ex)
            {
                // Exceptions: Unique id already exists or null object (not all data filled in)
                System.Diagnostics.Debug.WriteLine(ex.Message);

                if (ex.ParamName == "Classifiable.name")
                {
                    ObAddStatus.Text = FAIL_UNIQUE;
                }
                else if (ex.ParamName == "Classifiable.conceptStr")
                {
                    ObAddStatus.Text = FAIL_TERMS;
                }
                else
                {
                    ObAddStatus.Text = "Could not classify for some reason.";
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            catch (NullReferenceException ex)
            {
                ObAddStatus.Text = "Could not classify for some reason.";
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}