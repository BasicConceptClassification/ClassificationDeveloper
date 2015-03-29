using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using Neo4j;
using BCCLib;

namespace BCCApplication
{
    public partial class _Default : Page
    {
        private string DESCRIPTION = @"<p>Welcome to the Basic Concepts Classification. Here you can search for items found in the 
                                        galleries, archieves, and museums (GLAM). Each Item can be searched for by terms in the classification.</p>
                                        <p>Use the menu at the top of the to learn more about the Classification, start searching,
                                        or contact the admin to help classify GLAM objects today!</p>";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LabelDescription.Text = DESCRIPTION;

                // Testing purposes, only loading from BccRoot with a small depth
                int expandDepth = 2;
                
                // If server is down, just don't display anything
                try
                { 
                    // Fetch BCC from the DB
                    var dbConn = new Neo4jDB();
                    Term bccRootTerm = dbConn.getBccFromRootWithDepth(expandDepth);

                    // Create a starting TreeNode as the root to generate the BCC
                    TreeNode currentNode = new TreeNode();
                    DataSet.Nodes.Add(generateBccTree(bccRootTerm, currentNode));

                    // By default, leave collapsed
                    DataSet.CollapseAll();
                    DataSet.ShowCheckBoxes = TreeNodeTypes.Leaf;
                }
                catch { }
            }
        }


        protected void DataSet_SelectedNodeChanged(object sender, EventArgs e)
        {

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

        protected void SearchBtn_Click(object sender, EventArgs e)
        {
            string str = TextBox2.Text;
            Application["textpass"] = str;
            Response.Redirect("~/SearchResults.aspx", true);
        }
    }
}