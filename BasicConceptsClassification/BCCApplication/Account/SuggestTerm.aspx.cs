using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BCCLib;
using Neo4j;

//used for getting mail stuff done
using System.Net.Mail;
using System.Net;

namespace BCCApplication.Account
{
    public partial class SuggestTerm : System.Web.UI.Page
    {
        // How much the tree should be expanded by when it needs to expand.
        static int EXPAND_DEPTH = 2;
        private string ERROR_SERVER = "Sorry, there was an error with the server!";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                try { if (txtTermName.Text.Equals("")) throw new System.ArgumentException("Parameter cannot be null", "original"); }
                catch
                {
                    lblResult.Text = "Left 'Term Name parameter' blank!";
                    return;
                }
                try { if (txtParentString.Text.Equals("")) throw new System.ArgumentException("Parameter cannot be null", "original"); }
                catch
                {
                    lblResult.Text = "Left 'Parent String parameter' blank!";
                    return;
                }

                //Create the msg object to be sent
                MailMessage msg = new MailMessage();
                //Add your email address to the recipients
                msg.To.Add("bcclassification@gmail.com");
                //Configure the address we are sending the mail from
                MailAddress address = new MailAddress("bcclassification@gmail.com");
                msg.From = address;
                //Append their name in the beginning of the subject
                msg.Subject = "New Term Request";
                msg.Body = "There has been a request for the term: " + txtTermName.Text
                    + "\nAdd at location: " + txtParentString.Text
                    + "\nFor the reason: " + txtMessage.Text;

                //Configure an SmtpClient to send the mail.
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true; //only enable this if your provider requires it
                //Setup credentials to login to our sender email address ("UserName", "Password")
                NetworkCredential credentials = new NetworkCredential("bcclassification@gmail.com", "ZeroPunctuation");
                client.Credentials = credentials;

                //Send the msg
                client.Send(msg);

                //Display some feedback to the user to let them know it was sent
                lblResult.Text = "Your message was sent!";

                try
                {
                    // TODO: not hard code this!!!
                    var conn = new Neo4jDB();
                    conn.createNotification(
                        String.Format("{0}|{1}|{2}", txtTermName.Text, txtParentString.Text, txtMessage.Text), 
                        "bcclassification@gmail.com");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                //Clear the form
                txtTermName.Text = "";
                txtParentString.Text = "";
                txtMessage.Text = "";
            }
            catch
            {
                //If the message failed at some point, let the user know
                lblResult.Text = "Your message failed to send, please try again.";
            }
        }
    }
}