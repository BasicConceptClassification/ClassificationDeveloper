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
        private string FAIL_DEL_TERM = "Fail to delete the term.";

        /// <summary>
        /// load the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            ASTreeViewLinkNode asnode = new ASTreeViewLinkNode("","");
            astvMyTree.RootNode.AppendChild(generateASTree(bccRootTerm, asnode));
            DataSet.Nodes.Add(generateBccTree(bccRootTerm, currentNode));

            // By default, leave collapsed
            astvMyTree.GetCollapseAllScript();
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

        protected void AddUpdateButton_Click(object sender, EventArgs e)
        {
            //maynot use it 
        }

        /// <summary>
        /// For clicking the add button then will get the message for success or not
        /// </summary>
        /// <return> nothing.</return>
        protected void AddAddButton_Click(object sender, EventArgs e)
        {
            //get the string value from two text box
            string new_term_string = NewTermTextBox.Text;
            string parent_term = ParentTermTextBox.Text;

            //open the Neo4j database
            var conn = new Neo4jDB();

            //search the term by the rawterm
            Term searching_term = conn.getTermByRaw(parent_term);
            Term searching1_term = conn.getTermByRaw(new_term_string);

            if (searching1_term == null)
            {
                Term new_term = new Term
                {
                    id = new_term_string,
                    rawTerm = new_term_string,
                    lower = new_term_string.ToLower()
                };
                //conn.addTerm(new_term, searching_term);
                string teststring1 = "";

                //won't let the page crush

                try
                {
                    teststring1 = searching_term.ToString();
                }
                catch
                {

                }


                //return the result to let user know.

                if (teststring1 != "")
                {
                    conn.addTerm(new_term, searching_term);
                    Label1.Text = SUCCESS_ADD_TERM;
                    // Notify classifiers that a Term was created.
                    conn.createNotification(String.Format("Added new Term: {0}", new_term.rawTerm));

                }
                else
                {
                    Label1.Text = FAIL_ADD_TERM;
                }
            }
            else
            {
                Label1.Text = "term already have";
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
                Label2.Text = SUCCESS_MOVE_TERM;
            }
            else 
            {
                Label2.Text = FAIL_MOVE_TERM;
            }

               
            
        }
        protected void RenameUpdateButton_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// For clicking the rename button then will get the message for success or not
        /// </summary>
        /// <return> nothing.</return>
        protected void RenameAddButton_Click(Object sender, EventArgs e)
        {
            //get the string value from two text box
            string rename_from_string = RenameTermTextBox.Text;
            string rename_to_string = RenameToTextBox.Text;

            //open the Neo4j database
            var conn = new Neo4jDB();
            Term renameterm = conn.getTermByRaw(rename_from_string);
            string teststring1 = "";
            //won't let the page crush
            try
            {
                teststring1 = renameterm.ToString();
            }
            catch
            {

            }

            //set the input term to match with the term format in database
            renameterm.rawTerm = rename_to_string;
            renameterm.lower = rename_to_string.ToLower();

            //return the result to let user know.
            try{
                conn.renameTerm(renameterm, rename_to_string);
                Label3.Text = SUCCESS_RENAME_TERM;
                // Notify classifiers that a Term was renamed.
                conn.createNotification(String.Format("Renamed Term: {0} to {1}.", rename_from_string, rename_to_string));
            }
            catch
            {
                Label3.Text = FAIL_RENAME_TERM;
            }


        }

        /// <summary>
        /// For clicking the delete button then will get the message for success or not
        /// </summary>
        /// <return> nothing.</return>
        protected void DeleteUpdateButton_Click(object sender, EventArgs e)
        {
            //get the string value from text box
            string delete_term = DeleteTextBox.Text;

            //open the Neo4j database
            var conn = new Neo4jDB();
            Term delete_search_term = conn.getTermByRaw(delete_term);
            string teststring1 = "";
            //won't let the page crush\
            
            try
            {
                teststring1 = delete_search_term.ToString();
            }
            catch
            {

            }

            //return the result to let user know.
            if (teststring1 != "")
            {
                try
                {
                    conn.delTerm(delete_search_term);
                    Label4.Text = SUCCESS_DEL_TERM;
                    // Notify classifiers that a Term was deleted.
                    // TODO: create additional notification to the classifiers whose classifiable's ConStr
                    // were affect by the Term deletion.
                    conn.createNotification(String.Format("Removed Term: {0}", delete_term));
                }
                catch
                {
                    Label4.Text = "The term which you want to delete has sub terms so you can't delete this one"
                }
                
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