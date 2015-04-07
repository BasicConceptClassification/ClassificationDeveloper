using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Neo4j;
using BCCLib;
using Goldtect;
using Goldtect.Utilities.Xml;
using System.Web.Services;

namespace BCCApplication.Account
{
	public partial class AdminPage : System.Web.UI.Page
	{
        private string DESCRIPTION = "<p>You can edit the Classification by Adding, Moving, Renaming or Deleting a Term.</p>";

        /// <summary>
        /// load the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            //DataSet.Nodes.Add(generateBccTree(bccRootTerm, currentNode));

            // By default, leave collapsed
            //DataSet.CollapseAll();
            //DataSet.ShowCheckBoxes = TreeNodeTypes.Leaf;
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
            currentNode = new ASTreeViewLinkNode(nodeTerm,nodeTerm);

            // Sort subTerms by alphabetical order
            currentTerm.sortSubTerms();

            // Foreach child, recursively build this up
            foreach (var childTerm in currentTerm.subTerms)
            {
                currentNode.AppendChild(generateASTree(childTerm, currentNode));
            }
            return currentNode;
        }


        /// <summary>
        /// For clicking the move button then will get the message for success or not
        /// </summary>
        /// <return> nothing.</return>
        protected void MoveAddButton_Click(object sender, EventArgs e)
        {
            //get the string value from two text box
            string move_term1 = MoveTermTextBox.Text;
            string move_term2 = MoveTermUnderTextBox.Text;

            //open the Neo4j database
            var conn = new Neo4jDB();
            Term result_1 = conn.getTermByRaw(move_term1);
            Term result_2 = conn.getTermByRaw(move_term2);
            string teststring1 = "";
            string teststring2 = "";
            //won't let the page crush
            try
            {
                teststring1 = result_1.ToString();
            } 
            catch 
            {

            }
            //won't let the page crush
            try
            {
                teststring2 = result_2.ToString();
            }
            catch
            {

            }

            //return the result to let user know.
            if((teststring1 != "")&&(teststring2 !=""))
            {
                conn.moveTerm(result_1, result_2);
            }
               
            
        }
	}
}