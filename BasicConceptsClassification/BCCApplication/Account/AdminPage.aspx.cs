using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Neo4j;
using BCCLib;

namespace BCCApplication.Account
{
	public partial class AdminPage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
        {
		// Testing purposes, only loading from BccRoot with a small depth
            
            int expandDepth = 2;
            DataSet.Nodes.Clear();


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
        protected void AddUpdateButton_Click(object sender, EventArgs e)
        {


        }
        protected void AddAddButton_Click(object sender, EventArgs e)
        {
            /*
            string new_term_string = NewTermTextBox.Text;
            var conn = new Neo4jDB();
            Term new_term = new Term
            {
                id = new_term_string,
                rawTerm = new_term_string,
                lower = new_term_string.ToLower()
            };
            conn.addTerm(new_term, null);
             */
           /*
            string new_term_string = NewTermTextBox.Text;
            string parent_term = ParentTermTextBox.Text;
            var conn = new Neo4jDB();
            Term searching_term = conn.getTermByRaw(parent_term);
            Term new_term = new Term
            {
                id = new_term_string,
                rawTerm = new_term_string,
                lower = new_term_string.ToLower()
            };

            conn.addTerm(new_term, null);
            conn.moveTerm(new_term, searching_term);
            */

        }
        protected void MoveUpdateButton1_Click(object sender, EventArgs e)
        {


        }
        protected void MoveUpdateButton2_Click(object sender, EventArgs e)
        {


        }
        protected void MoveAddButton_Click(object sender, EventArgs e)
        {
            /*
            string move_term1 = MoveTermTextBox.Text;
            string move_term2 = MoveTermUnderTextBox.Text;
            var conn = new Neo4jDB();
            Term result_1 = conn.getTermByRaw(move_term1);
            Term result_2 = conn.getTermByRaw(move_term2);
            conn.moveTerm(result_1, result_2);
            */
        }
        protected void RenameUpdateButton_Click(object sender, EventArgs e)
        {

        }
        protected void RenameAddButton_Click(Object sender, EventArgs e)
        {

        }

        protected void DeleteUpdateButton_Click(object sender, EventArgs e)
        {

        }

        protected void DeleteSafelyButton_Click(object sender, EventArgs e)
        {

        }

        protected void DeleteOverwriteButton_Click(object sender, EventArgs e)
        {

        }
	}
}