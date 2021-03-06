﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCCLib;
using Neo4j;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BCCApplication.Account
{

    
    public partial class EditClassifiable : System.Web.UI.Page
    {
        // How much the tree should be expanded by when it needs to expand.
        static int EXPAND_DEPTH = 2;

        private string DESCRIPTION = @"<p>To edit any of GLAM Objects you have permission to classify, first select them from one of
                                        the two lists below: those that are classified, and those that are not classified, and then click
                                        The button below the list to get their information. Then once their information is shown in the boxes
                                        above the lists, then you can edit any of the GLAM Object's information. Once you are done,
                                        click the Edit button.</p>";

        static string select_string;
        private string SUCCESS = "Successfully edited the classifiable.";
        private string FAIL = "Failed to edit the classifiable. ";
        private string FAIL_UNIQUE = "Failed: Another GLAM Object with that name already exists in your GLAM.";
        protected const string ERROR_SERVER = "Sorry, error with the server!";

        private static IEnumerable<Classifiable> unclassifieds;
        //private static ClassifiableCollection unclassifieds = new ClassifiableCollection
        //{
       //     data = new List<Classifiable>(),
        //};
        private static ClassifiableCollection classifieds = new ClassifiableCollection
        {
            data = new List<Classifiable>(),
        };

        /// <summary>
        /// when the page load it display the classified and unclassified elements on two lists.
        /// </summary>
        /// <return> nothing will return </returns>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LabelDescription.Text = DESCRIPTION;

                // Get the logged in user's email
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var currentUser = manager.FindById(User.Identity.GetUserId());
                string userEmail = currentUser.Email;

                var dbConn = new Neo4jDB();
                
                GetClassifieds(dbConn, userEmail);
                GetUnclassifieds(dbConn, userEmail);
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

        /// <summary>
        /// Clears all the textbox fields.
        /// </summary>
        protected void ClearFields()
        {
            TextBox_Name.Text = "";
            TextBox_URL.Text = "";
            TextBox_Concept.Text = "";
        }

        protected void GetUnclassifieds(Neo4jDB dbConn, string classifierEmail)
        {
            // Not the greatest, but the page will not crash at least
            try
            {
                ListBox2.Items.Clear();
                ClassifiableCollection adminModified = dbConn.getAllAdminModified(classifierEmail);
                ClassifiableCollection justUnclassifieds = dbConn.getAllUnclassified(classifierEmail);
                
                // Clean up later if time; for now, appending the admin modified stuff.
                if (adminModified.data.Count > 0)
                {
                    unclassifieds = justUnclassifieds.data.Concat(adminModified.data);
                }
                else
                {
                    unclassifieds = justUnclassifieds.data;
                }

                int resultsLength = unclassifieds.Count();
                for (int i = 0; i < resultsLength; i++)
                {
                    Classifiable currentUNClassifiable = unclassifieds.ElementAt(i);
                    ListBox2.Items.Add(currentUNClassifiable.name);
                }
                if (resultsLength > 0) ListBox2.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Label1.Text = ERROR_SERVER;
            }
        }

        protected void GetClassifieds(Neo4jDB dbConn, string classifierEmail)
        {
            // Not the greatest, but the page will not crash at least
            try
            {
                ListBoxClass.Items.Clear();
                classifieds = dbConn.getAllClassified(classifierEmail);
                int resultLength = classifieds.data.Count;
                for (int i = 0; i < resultLength; i++)
                {
                    Classifiable currentClassifiable = classifieds.data[i];
                    ListBoxClass.Items.Add(currentClassifiable.name);
                }
                if (resultLength > 0) ListBoxClass.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Label1.Text = ERROR_SERVER;
            }
        }
        

        /// <summary>
        /// click the Edit button and to edite the current classified or unclassified elements
        /// </summary>
        /// <return> nothing will return </returns>
        protected void Edit_Click(object sender, EventArgs e)
        {
            string str_n = TextBox_Name.Text;
            string str_u = TextBox_URL.Text;
            string str_c = TextBox_Concept.Text;

            //-----------------------------set the new classifiable------------------------------------
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;

            var conn = new Neo4jDB();
            GLAM gl = conn.getGlamOfClassifier(userEmail);


            string username = Context.GetOwinContext().Authentication.User.Identity.Name;
            string email = userEmail;
            Classifier classifier = new Classifier(gl, email, username);

            string trimConceptString = str_c.Trim();

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
            string the_status;
            if (str_c != "")
            {
                the_status = Classifiable.Status.Classified.ToString();
            }
            else{
                the_status = Classifiable.Status.Unclassified.ToString();
            }

            //setting the new classifiable
            Classifiable newClassifiable = new Classifiable
            {
                id = gl.name + "_" + str_n,
                name = str_n,
                url = str_u,
                perm =  EditPerm.SelectedValue,
                status = the_status,
                owner = classifier,
                conceptStr = newConceptStr,
            };

            Classifiable matchedClassifiable = conn.getClassifiableById(gl.name+ "_" + select_string);

            //check is there have any concept string
            string teststr = "";
            
            try
            {
                teststr = matchedClassifiable.conceptStr.ToString();
            }
            catch
            {

            }

            //update the edited classifiable
            //and return the message to user.
            try
            {
                conn.updateClassifiable(matchedClassifiable, newClassifiable, classifier);
                Label1.Text = SUCCESS;
                // Update these two lists
                GetUnclassifieds(conn, userEmail);
                GetClassifieds(conn, userEmail);
                ClearFields();
            }
            catch (ArgumentException ex)
            {
                // Exceptions: Unique id already exists or null object (not all data filled in)
                System.Diagnostics.Debug.WriteLine(ex.Message);

                if (ex.ParamName == "Classifiable.name")
                {
                    Label1.Text = FAIL_UNIQUE;
                }
                else if (ex.ParamName == "Classifiable.conceptStr")
                {
                    // really want the paramter name, but not sure how JUST get the message...
                    // This error message will print out what terms are missing!
                    string extractMeOut = "Parameter Name: " + ex.ParamName;
                    Label1.Text = ex.Message.Substring(0, ex.Message.Length - extractMeOut.Length);
                }
                else
                {
                    Label1.Text = FAIL;
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }


        /// <summary>
        /// click the update button to pop the choosen elment's detials onto textbox
        /// </summary>
        /// <return> nothing will return </returns>
        protected void Update_Class_Click(object sender, EventArgs e)
        {
            // Clear the notification text
            Label1.Text = "";
            if (classifieds.data.Count > 0)
            {
                select_string = "";
                select_string = ListBoxClass.SelectedItem.ToString();
                TextBox_Name.Text = select_string;
                var dbConn = new Neo4jDB();

                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var currentUser = manager.FindById(User.Identity.GetUserId());
                string userEmail = currentUser.Email;

                //and return the message to user.
                try
                {
                    // Use the ids that were grabbed earlier to fill in the fields
                    Classifiable matchedClassifiable = dbConn.getClassifiableById(classifieds.data[ListBoxClass.SelectedIndex].id);
                    TextBox_Name.Text = matchedClassifiable.name;
                    TextBox_URL.Text = matchedClassifiable.url;
                    TextBox_Concept.Text = matchedClassifiable.conceptStr.ToString();
                    EditPerm.SelectedValue = matchedClassifiable.perm.ToString();
                }
                catch
                {
                    Label1.Text = "Please choose one of the classifiable element in the list.";
                    // Refresh lists just in case they changed
                    GetUnclassifieds(dbConn, userEmail);
                    GetClassifieds(dbConn, userEmail);
                }
            }
        }

        /// <summary>
        /// click the update button to pop the choosen elment's detials onto textbox
        /// </summary>
        /// <return> nothing will return </returns>
        protected void Update_Unclass_Click(object sender, EventArgs e)
        {
            // Clear the notification text
            Label1.Text = "";
            if (unclassifieds.Count() > 0)
            {
                select_string = "";
                //var selected = list
                select_string = ListBox2.SelectedItem.ToString();
                //TextBox_Name.Text = select_string;
                var dbConn = new Neo4jDB();

                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var currentUser = manager.FindById(User.Identity.GetUserId());
                string userEmail = currentUser.Email;

                var conn = new Neo4jDB();

                //and return the message to user.
                try
                {
                    // Use the ids that were grabbed earlier
                    Classifiable matchedClassifiable = dbConn.getClassifiableById(unclassifieds.ElementAt(ListBox2.SelectedIndex).id);
                    TextBox_Name.Text = select_string;
                    TextBox_URL.Text = matchedClassifiable.url;
                    TextBox_Concept.Text = matchedClassifiable.conceptStr.ToString();
                    EditPerm.SelectedValue = matchedClassifiable.perm.ToString();
                }
                catch
                {
                    Label1.Text = "Please choose one of the classifiable element in the list.";
                    // Refresh lists just in case they changed
                    GetUnclassifieds(dbConn, userEmail);
                    GetClassifieds(dbConn, userEmail);
                }
            }
        }
    }
}