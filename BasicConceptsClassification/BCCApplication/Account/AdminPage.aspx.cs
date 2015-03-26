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

        private string SUCCESS_ADD_TERM = "Successfully added the term.";
        private string SUCCESS_MOVE_TERM = "Successfully move the term.";
        private string SUCCESS_RENAME_TERM = "Successfully rename the term.";
        private string SUCCESS_DEL_TERM = "Successfully delete  the term.";
        private string FAIL_ADD_TERM = "Fail added the term.";
        private string FAIL_MOVE_TERM = "Fail move the term.";
        private string FAIL_RENAME_TERM = "Fail to rename the term.";
        private string FAIL_DEL_TERM = "Fail to delete  the term.";

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
            //maynot use it 
        }

        protected void AddAddButton_Click(object sender, EventArgs e)
        {
          
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
            string teststring1 = "";
            //TODO:need to tell the user.
            try
            {
                teststring1 = searching_term.ToString();
            }
            catch
            {

            }

            if (teststring1 != "")
            {
                conn.addTerm(new_term, searching_term);
                Label1.Text = SUCCESS_ADD_TERM;
               
            }
            else
            {
                Label1.Text = FAIL_ADD_TERM;
            }
           
            

        }

        protected void MoveUpdateButton1_Click(object sender, EventArgs e)
        {
            //maynot use it 
        }
        protected void MoveUpdateButton2_Click(object sender, EventArgs e)
        {
            //maynot use it 
        }
        protected void MoveAddButton_Click(object sender, EventArgs e)
        {
            
            string move_term1 = MoveTermTextBox.Text;
            string move_term2 = MoveTermUnderTextBox.Text;
            var conn = new Neo4jDB();
            Term result_1 = conn.getTermByRaw(move_term1);
            Term result_2 = conn.getTermByRaw(move_term2);
            string teststring1 = "";
            string teststring2 = "";
            //TODO:need to tell the user.
            try
            {
                teststring1 = result_1.ToString();
            } 
            catch 
            {

            }
            //TODO:need to tell the user.
            try
            {
                teststring2 = result_2.ToString();
            }
            catch
            {

            }
            //TODO:need to tell the user.
            if((teststring1 != "")&&(teststring2 !=""))
            {
                conn.moveTerm(result_1, result_2);
                Label2.Text = SUCCESS_MOVE_TERM;
            }
            else 
            {
                Label2.Text = FAIL_MOVE_TERM;
            }

               
            
        }
        protected void RenameUpdateButton_Click(object sender, EventArgs e)
        {
            //maynot use it 
        }
        protected void RenameAddButton_Click(Object sender, EventArgs e)
        {
            string rename_from_string = RenameTermTextBox.Text;
            string rename_to_string = RenameToTextBox.Text;
            var conn = new Neo4jDB();
            Term renameterm = conn.getTermByRaw(rename_from_string);
            string teststring1 = "";
            string teststring2 = "";
            //TODO:need to tell the user.
            try
            {
                teststring1 = renameterm.ToString();
            }
            catch
            {

            }

            renameterm.rawTerm = rename_to_string;
            renameterm.lower = rename_to_string.ToLower();

            //TODO:need to tell the user.
            //when didn't enter anything
            if ((teststring1 != "") && (teststring2 != ""))
            {
                conn.renameTerm(renameterm, rename_to_string);
                Label3.Text = SUCCESS_RENAME_TERM;
            }
            else
            {
                Label3.Text = FAIL_RENAME_TERM;
            }


        }

        protected void DeleteUpdateButton_Click(object sender, EventArgs e)
        {
            string delete_term = DeleteTextBox.Text;
            var conn = new Neo4jDB();
            Term delete_search_term = conn.getTermByRaw(delete_term);
            string teststring1 = "";
            //TODO:need to tell the user.
            try
            {
                teststring1 = delete_search_term.ToString();
            }
            catch
            {

            }

            if (teststring1 != "")
            {
                conn.delTermFORCE(delete_search_term);
                Label4.Text = SUCCESS_DEL_TERM;
            }
            else
            {
                Label4.Text = FAIL_DEL_TERM;
            }
            
        }

        protected void DeleteSafelyButton_Click(object sender, EventArgs e)
        {

        }

        protected void DeleteOverwriteButton_Click(object sender, EventArgs e)
        {

        }
	}
}