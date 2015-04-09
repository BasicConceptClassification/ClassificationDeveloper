using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using Neo4j;
using BCCLib;

using Goldtect;
using Goldtect.Utilities.Xml;

namespace BCCApplication
{
    public partial class _Default : Page
    {
        // How much the tree should be expanded by when it needs to expand.
        private string ERROR_SERVER = "Sorry, there was an error with the server!";

        private string DESCRIPTION = @"<p>Here on the search page, you can enter in Terms found in the Controlled Vocabulary
                                    to search for items found in GLAMs. Once you have entered the Terms you want to search
                                    by, click the Jump to Search Page button to go to the Search Results. There you will
                                    have the option to choose what search options you want before the search starts.</p>";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LabelDescription.Text = DESCRIPTION;
                CreateTree();
            }
        }


        /// <summary>
        /// Creates the Controlled Vocabulary as a Tree.
        /// </summary>
        protected void CreateTree()
        {
            // Testing purposes, only loading from BccRoot with a small depth
            //DataSet.Nodes.Clear();
            astvMyTree.RootNode.Clear();

            // Fetch BCC from the DB
            var dbConn = new Neo4jDB();
            Term bccRootTerm = dbConn.getBccFromRootWithDepth(-1);

            // Create a starting TreeNode as the root to generate the BCC
            ASTreeViewLinkNode asnode = new ASTreeViewLinkNode("", "");
            astvMyTree.RootNode.AppendChild(generateASTree(bccRootTerm, asnode));
            astvMyTree.CollapseAll();
        }

        /// <summary>
        /// Converts a Term and its subTerms/children Terms to a TreeNode. 
        /// </summary>
        /// <param name="currentTerm">Current Term in the recursion.</param>
        /// <param name="currentNode">Current TreeNode in the recursion.</param>
        /// <returns>TreeNode reprentation of a Term.</returns>

        protected ASTreeViewLinkNode generateASTree(Term currentTerm, ASTreeViewLinkNode currentNode)
        {
            String nodeTerm = currentTerm.rawTerm;

            // Create/set the current node we're on: the text displayed with the
            // node and the action when selected.
            currentNode = new ASTreeViewLinkNode(nodeTerm, nodeTerm);

            // Sort subTerms by alphabetical order
            currentTerm.sortSubTerms();

            // Foreach child, recursively build this up
            foreach (var childTerm in currentTerm.subTerms)
            {
                currentNode.AppendChild(generateASTree(childTerm, currentNode));
            }
            return currentNode;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string str = TextBox2.Text;
            Application["textpass"] = str;
            Response.Redirect("SearchResults.aspx", true);
        }
    }
}