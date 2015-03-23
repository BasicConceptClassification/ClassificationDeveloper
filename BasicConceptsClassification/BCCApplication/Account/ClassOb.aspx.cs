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
        private string SUCCESS_ADD = "Successfully added: ";
        private string FAIL_UNIQUE = "Failed: Another GLAM Object with that name already exists in your GLAM.";
        private string FAIL_TERMS = "Not all the terms in the concept string are from the controlled vocabulary.";
        static int counter_once = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (counter_once == 1)
            {
                string getinput_name = "";
                string getinput_url = "";
                string getinput_con = "";

                try
                {
                    getinput_name = Application["namepass"].ToString();
                    getinput_url = Application["urlpass"].ToString();
                    getinput_con = Application["conpass"].ToString();
                }
                catch
                {

                }
                ObName.Text = getinput_name;
                ObURL.Text = getinput_url;
                ObConcept.Text = getinput_con;
                counter_once--;

            }
            if (!Page.IsPostBack)
            {
                // At the moment, generates another tree after clicking the Submit button.
                int expandDepth = 3;

                // Fetch BCC from the DB
                var dbConn = new Neo4jDB();

                // Fix this to be a bit better for catching and handling error messages
                try
                {
                    Term bccRootTerm = dbConn.getBccFromRootWithDepth(expandDepth);

                    // Create a starting TreeNode as the root to generate the BCC
                    TreeNode currentNode = new TreeNode();
                    DataSet.Nodes.Add(generateBccTree(bccRootTerm, currentNode));

                    // By default, leave collapsed
                    DataSet.CollapseAll();
                    DataSet.ShowCheckBoxes = TreeNodeTypes.All;
                }
                catch
                {
                }
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

            // TODO: cleanup. It's a bit...messy.
            var conn = new Neo4jDB();
            GLAM gl = conn.getGlamOfClassifier(userEmail);

            Classifier classifier = new Classifier(gl);
            classifier.name = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = userEmail;

            // TODO: either make a constructor for ConceptString to take (this)(format) and have it parse
            // it out so we don't have to see this parsing every single time AND create terms from it?
            //split the input concept string from (xx)(xx)(xx) to a list without () 
            string trimConceptString = inputConcept.Trim();

            //convert the string list to the term list
            List<Term> newTerms = new List<Term>();
            
            // Only extract terms if there are any terms added
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

            // TODO: GET PROPER VALUES:
            // id - created by "<GLAM_NAME>_<CLASSIFIABLE_NAME>
            Classifiable newClassifiable = new Classifiable
            {
                id = classifier.name + "_" + inputName,
                name = inputName,
                url = inputUrl,
                perm = EditPerm.SelectedValue,
                owner = classifier,
                conceptStr = newConceptStr,
            };

            // Not entirely true but it's better than it was before
            // TODO: account for other "statuses" of being unclassified.
            if (newClassifiable.conceptStr.ToString() == "")
            {
                newClassifiable.status = Classifiable.Status.Unclassified.ToString();
            }
            else
            {
                newClassifiable.status = Classifiable.Status.Classified.ToString();
            }

            System.Diagnostics.Debug.WriteLine(String.Format("ClassOb_GOT: name: {0}; url: {1}; perm: {2}; conStr: {3}; ownerEmail: {4};", 
                newClassifiable.name, newClassifiable.url, newClassifiable.perm, newClassifiable.conceptStr.ToString(), newClassifiable.owner.email));

            // Try to add the newClassifiable and display an error message depending on the result.
            try
            {
                System.Diagnostics.Debug.WriteLine(String.Format("ClassOb_TRYING: Attempting to add Classifiable with name {0}", newClassifiable.name));
                Classifiable result = conn.addClassifiable(newClassifiable);

                if (result != null)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("ClassOb_SUCCESS: Added Classifiable with name {0}", newClassifiable.name));
                    ObAddStatus.Text = String.Format("{0} {1}.", SUCCESS_ADD, result.name);
                }

            }
            catch (ArgumentException ex)
            {
                // Exceptions: Unique id already exists, null object (not all data filled in)
                // Do some exception handling based on Exception type ...learn how to do custom exceptions?
                System.Diagnostics.Debug.WriteLine(String.Format("ClassOb_FAILED: could not add Classifiable with name {0}", newClassifiable.name));
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

            // Foreach child, recursively build this up
            foreach (var childTerm in currentTerm.subTerms)
            {
                currentNode.ChildNodes.Add(generateBccTree(childTerm, currentNode));
            }
            return currentNode; 
        }
             
    }



}