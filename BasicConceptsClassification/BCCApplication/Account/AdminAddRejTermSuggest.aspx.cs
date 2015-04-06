using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BCCLib;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Neo4j;

namespace BCCApplication.Account
{
    public partial class AdminAddRejTermSuggest : System.Web.UI.Page
    {
        // How much the tree should be expanded by when it needs to expand.
        static int EXPAND_DEPTH = 2;

        protected static List<Neo4jNotification> notifications = new List<Neo4jNotification>();

        protected const string DESCRIPTION = @"<p>If you have any suggested terms, you can view them below.</p>
                                                <p>In order to accept or reject a suggested term, you must first select a term
                                                from the list below and click <em>View Information</em>. Once the information is
                                                displayed below, then you can choose to accept or reject the suggested term.</p>
                                                <p>If a new term is added, all Classifiers will be notified.</p>";

        protected static char SEPARATOR_CHAR = '|';

        protected const string SUGGESTIONS_NONE = "You have no suggested terms to look at.";

        protected const string SUCCESS_ACCEPT = "Successfully Added the Suggested Term.";
        protected const string SUCCESS_REJECT = "Rejected Suggested Term.";
        protected const string ERROR_PARSE = "Could not parse out individual parts.";
        protected const string ERROR_PARSE_INSTRUCTIONS = @"There was a paring error. If you wish to add the term to the BCC, 
                                                        you will have to reject this suggestion and add it manually.
                                                        Please review your email notification for the exact information about this suggestion.";
        protected const string ERROR_SERVER = "Sorry, error with the server!";

        protected const string ERROR_NOT_SELECTED = "Please select a term.";

        protected const string ERROR_FIND_PARENT = "Could not find the parent term. You may have to remove the notification and add it manually.";
        protected const string ERROR_ADD = "Could not add the term to the BCC. You may have to remove the notification and add it manually.";
        protected const string ERROR_ADD_DUPLICATE = "This Term already exists in the BCC.";

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                LabelDescription.Text = DESCRIPTION;
                GetNotifications();
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

        protected void ClearFields()
        {
            txtTermName.Text = "";
            txtParentString.Text = "";
            txtMessage.Text = "";
            curNoticationIndex.Value = "";
        }

        protected void GetNotifications()
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;

            try
            {
                var conn = new Neo4jDB();
                notifications = conn.getNotifications(userEmail);

                // Only display the suggested term listbox if notifications exist
                if (notifications.Count > 0)
                {
                    ListBoxClass.Items.Clear();
                    foreach (var note in notifications)
                    {
                        string[] tokens = note.msg.Split(SEPARATOR_CHAR);

                        // Just in case the parse token gets changed or something,
                        // something will still be displayed. Of course, this won't
                        // help the adding or removal because it will still try to parse
                        // by the same separator
                        if (tokens.Count() == 3)
                        {
                            ListBoxClass.Items.Add(tokens[0]);
                        }
                        else
                        {
                            ListBoxClass.Items.Add(note.msg);
                        }
                    }
                    ListBoxClass.SelectedIndex = 0;
                    ListBoxClass.Visible = true;
                    Update_SuggTerm.Visible = true;
                    LabelSuggestedTerms.Text = "";
                }
                else
                {
                    ListBoxClass.Visible = false;
                    Update_SuggTerm.Visible = false;
                    LabelSuggestedTerms.Text = SUGGESTIONS_NONE;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        
        /// <summary>
        /// Attempts to add the term in the notification.
        /// </summary>
        /// <returns>0 is a successful add, 1 is failure to add.</returns>
        protected int AddNewTerm()
        {
            // If there was an error parse from earlier, don't continue
            if (txtTermName.Text == ERROR_PARSE)
            {
                lblResult.Text = ERROR_PARSE_INSTRUCTIONS;
                return 1;
            }

            int indexToAdd = Convert.ToInt32(curNoticationIndex.Value);

            var conn = new Neo4jDB();
            Term parent = new Term();

            // Try to see if we can find the parent
            try
            {
                parent = conn.getTermByRaw(txtParentString.Text);
            }

            catch (Exception Ex)
            {
                lblResult.Text = ERROR_SERVER;
                System.Diagnostics.Debug.WriteLine(Ex.Message);
                return 1;
            }

            if (parent == null)
            {
                lblResult.Text = ERROR_FIND_PARENT;
                return 1;
            }

            Term newChildTerm = new Term
                {
                    id = txtTermName.Text,
                    rawTerm = txtTermName.Text,
                    lower = txtTermName.Text.ToLower(),
                };

            // Try to see if we can add the term
            try
            {
                conn.addTerm(newChildTerm, parent);
            }
            catch (Exception Ex)
            {
                if (Ex.Message.Contains("id"))
                {
                    lblResult.Text = ERROR_ADD_DUPLICATE;
                }
                else
                {
                    lblResult.Text = ERROR_ADD;
                    System.Diagnostics.Debug.WriteLine(Ex.Message);
                }
                return 1;
            }

            // Send a notification to all classifiers
            try
            {
                conn.createNotification(String.Format("Added new Term: {0}", newChildTerm.rawTerm));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            lblResult.Text = SUCCESS_ACCEPT;
            return 0;
        }

        /// <summary>
        /// Removes the notification from the list of suggested term notifications.
        /// </summary>
        /// <returns>>0 is a successful remove, 1 is failure to remove.</returns>
        protected int RemoveNotification()
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;

            int indexToRm = Convert.ToInt32(curNoticationIndex.Value);

            // Remove notification
            try
            {
                var conn = new Neo4jDB();
                conn.removeNotification(userEmail, notifications[indexToRm]);

                // Update Notifications List and Clear fields
                GetNotifications();
                ClearFields();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                lblResult.Text = ERROR_SERVER;
                return 1;
            }
            return 0;
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            if (curNoticationIndex.Value != "")
            {
                // Try to add term
                // if term add is successful, then remove notification
                if (AddNewTerm() == 0)
                {
                    RemoveNotification();
                }
            }
            else
            {
                lblResult.Text = ERROR_NOT_SELECTED;
            }

        }
        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (curNoticationIndex.Value != "")
            {
                // the result text will be updated to some error inside the function
                if (RemoveNotification() == 0)
                {
                    lblResult.Text = SUCCESS_REJECT;
                }
            }
            else
            {
                lblResult.Text = ERROR_NOT_SELECTED;
            }
        }

        /// <summary>
        /// Format: X|Y|Z
        /// </summary>
        /// <param name="notification"></param>
        protected void parseAndSetNotification(Neo4jNotification notification, int index)
        {
            string [] tokens = notification.msg.Split(SEPARATOR_CHAR);

            // If the separation goes wonky, then at least the full message would
            // appear in the description.
            if (tokens.Count() == 3)
            {
                txtTermName.Text = tokens[0];
                txtParentString.Text = tokens[1];
                txtMessage.Text = tokens[2];
                lblResult.Text = "";
            }
            else
            {
                txtTermName.Text = ERROR_PARSE;
                txtParentString.Text = ERROR_PARSE;
                txtMessage.Text = notification.msg;
                lblResult.Text = ERROR_PARSE_INSTRUCTIONS;
            }

            // And set these so we have the correct notification values at the very least
            curNoticationIndex.Value = index.ToString();
        }

        /// <summary>
        /// Get the notification message information into the info boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Update_SuggTerm_Click(object sender, EventArgs e)
        {
            // Just in case...
            if (notifications.Count != 0)
            {
                lblResult.Text = "";
                int selectedIndex = ListBoxClass.SelectedIndex;
                parseAndSetNotification(notifications[selectedIndex], selectedIndex);
            }
        }
    }
}