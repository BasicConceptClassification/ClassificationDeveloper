using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCCLib;
using Neo4j;
using System.Web.Security;

using System.Diagnostics;

namespace BCCApplication.Account
{
    public partial class ClassOb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
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

        //the control submit button
        protected void SubmitObj_Click(object sender, EventArgs e)
        {
            //input of each text box
            //get vaule of each
            string inputUrl = ObURL.Text;
            string inputName = ObName.Text;
            string inputConcept = ObConcept.Text;

            // create a temp GALM for testing
            // TODO: fetch proper GLAM
            GLAM gl = new GLAM("UA");

            // TODO: Fetch email properly
            Classifier classifier = new Classifier(gl);
            classifier.name = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = "somewhere@com";

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

            var conn = new Neo4jDB();

            // Try to add the newClassifiable and display an error message depending on the result.
            try
            {
                Classifiable result = conn.addClassifiable(newClassifiable);

                if (result != null)
                {
                    ObAddStatus.Text = String.Format("Successfully added {0}.", result.name);
                }

            }
            catch (Exception ex)
            {
                // Exceptions: Unique id already exists, null object (not all data filled in)
                // Do some exception handling based on Exception type ...learn how to do custom exceptions?
                Debug.WriteLine(ex.Message);
                ObAddStatus.Text = String.Format("Could not add because {0}.", "REASONS");
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