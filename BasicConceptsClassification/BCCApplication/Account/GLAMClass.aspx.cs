using System;
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
    public partial class GLAMClass : System.Web.UI.Page
    {
        private string DESCRIPTION = @"<p>Below you can see your notifications, your recently classified GLAM objects, those not classified,
                                        or those that might need some special attention.
                                        You can also Add new GLAM objects to your GLAM or remove any GLAM objects that you have added.</p>";
        
        private static int NUM_RECENT_CLASSIFIED = 10;
        private string YOUR_RECENTLY_CLASSIFIED = String.Format("<p>Here are your {0:D} (at most) recently classified GLAM Objects.</p>", NUM_RECENT_CLASSIFIED);

        private string NOTIFICATIONS_NONE = "You have no notifications.";
        private string RECENTCLASSIFIED_NONE = "No GLAM objects have been recently classified.";
        private string UNCLASSFIED_NONE = "All your GLAM objects are classified!";
        private string UNCLASSFIED_SPECIAL_NONE = "No GLAM Objects require special attention!";
        private string ERROR_SERVER = "Having server issues, sorry!";

        static Neo4jDB dbConn = new Neo4jDB();
        static string userEmail = ""; 
        static List<Neo4jNotification> notifications = new List<Neo4jNotification>();

        protected void Setup_WebContent()
        {
            LabelDescription.Text = DESCRIPTION;
            LabelDescRecClassObj.Text = YOUR_RECENTLY_CLASSIFIED;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get Any modifiable content ready!
            Setup_WebContent();

            // Manager was from register.aspx page
            // Ref: http://blogs.msdn.com/b/webdev/archive/2013/10/16/customizing-profile-information-in-asp-net-identity-in-vs-2013-templates.aspx
            // See "Getting Profile Information"
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            userEmail = currentUser.Email;

            // Generate all the tables that appear on the page
            GenerateNotifications(dbConn, userEmail);
            GenerateRecentlyClassified(dbConn, userEmail, TableRecClassObj);
            GenerateUnclassified(dbConn, userEmail, TableUnClassList);
            GenerateUnclassifiedSpecial(dbConn, userEmail, TableUnClassAdminCause);
        }

        /// <summary>
        /// Fetchs and displays notifications for the currently logged in user.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="classifierEmail"></param>
        protected void GenerateNotifications(Neo4jDB conn, string classifierEmail)
        {
            try
            {
                notifications = conn.getNotifications(classifierEmail);
                
                if (notifications.Count > 0)
                {
                    // Remove the old stuff
                    TableNotification.Rows.Clear();

                    // Add the header row/cells
                    TableRow tRowHead = new TableHeaderRow();

                    TableCell tCellHead = new TableHeaderCell();
                    tCellHead.Text = "Message";
                    tRowHead.Cells.Add(tCellHead);

                    tCellHead = new TableHeaderCell();
                    tCellHead.Text = "Action";
                    tRowHead.Cells.Add(tCellHead);

                    TableNotification.Rows.Add(tRowHead);

                    // Add the regular rows/cells
                    for (int i = 0; i < notifications.Count; i++)
                    {
                        TableRow tRow = new TableRow();
                        TableCell tCell = new TableCell();

                        tCell.Text = notifications[i].msg;
                        tRow.Cells.Add(tCell);

                        tCell = new TableCell();

                        Button btn = new Button();
                        btn.Text = "Delete";
                        btn.ID = i.ToString();
                        btn.Click += new EventHandler(btnDeleteNotification_Click);
                        
                        tCell.Controls.Add(btn);

                        tRow.Cells.Add(tCell);

                        TableNotification.Rows.Add(tRow);
                    }

                    TableNotification.Visible = true;
                    LabelTableNotification.Visible = false;
                }
                else
                {
                    LabelTableNotification.Visible = true;
                    LabelTableNotification.Text = NOTIFICATIONS_NONE;
                    TableNotification.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GLAMClass_Exception: {0}", ex.Message);

                LabelTableNotification.Visible = true;
                LabelTableNotification.Text = ERROR_SERVER;
                TableNotification.Visible = false;
            }
        }

        /// <summary>
        /// Fetches and fills in the classifier's recently classified GLAM objects,
        /// ordered by most recent.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="classifierEmail"></param>
        protected void GenerateRecentlyClassified(Neo4jDB conn, string classifierEmail, Table ClassifableTable)
        {
            try
            {
                ClassifiableCollection recentClassifiables = conn.getRecentlyClassified(classifierEmail, NUM_RECENT_CLASSIFIED);
                
                if (recentClassifiables.data.Count > 0)
                {
                    // Remove the old stuff
                    ClassifableTable.Rows.Clear();

                    // Add the header row/cells
                    TableRow tRowHead = new TableHeaderRow();

                    TableCell tCellHead = new TableHeaderCell();
                    tCellHead.Text = "Name";
                    tRowHead.Cells.Add(tCellHead);

                    tCellHead = new TableHeaderCell();
                    tCellHead.Text = "Concept String";
                    tRowHead.Cells.Add(tCellHead);

                    tCellHead = new TableHeaderCell();
                    tCellHead.Text = "URL";
                    tRowHead.Cells.Add(tCellHead);

                    tCellHead = new TableHeaderCell();
                    tCellHead.Text = "Last Modified By";
                    tRowHead.Cells.Add(tCellHead);

                    ClassifableTable.Rows.Add(tRowHead);

                    foreach (Classifiable recent in recentClassifiables.data)
                    {
                        TableRow tRow = new TableRow();

                        // Name
                        TableCell tCell = new TableCell();
                        tCell.Text = recent.name;
                        tRow.Cells.Add(tCell);

                        // Concept String
                        tCell = new TableCell();
                        tCell.Text = recent.conceptStr.ToString();
                       // tCell.Text = "Coming Soon!";
                        tRow.Cells.Add(tCell);

                        // Url
                        tCell = new TableCell();
                        tCell.Text = recent.url;
                        tRow.Cells.Add(tCell);

                        // Last Modified By
                        tCell = new TableCell();
                        tCell.Text = recent.classifierLastEdited.ToString();
                        //tCell.Text = " Also Coming Soon!";
                        tRow.Cells.Add(tCell);

                        ClassifableTable.Rows.Add(tRow);
                    }
                    LabelRecClassObj.Visible = false;
                    ClassifableTable.Visible = true;
                }
                else
                {
                    LabelRecClassObj.Visible = true;
                    LabelRecClassObj.Text = RECENTCLASSIFIED_NONE;
                    ClassifableTable.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GLAMClass_Exception: {0}", ex.Message);

                LabelRecClassObj.Visible = true;
                LabelRecClassObj.Text = ERROR_SERVER;
                ClassifableTable.Visible = false;
            }
        }

        /// <summary>
        /// Fetches and fills in the unclassified classifiables that the Classifier has persmission to access.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="classifierEmail"></param>
        protected void GenerateUnclassified(Neo4jDB conn, string classifierEmail, Table ClassifableTable)
        {
            try
            {
                ClassifiableCollection classifiables = conn.getAllUnclassified(classifierEmail);
                if (classifiables.data.Count > 0)
                {
                    // Remove the old stuff
                    ClassifableTable.Rows.Clear();

                    // Add the header row/cells
                    TableRow tRowHead = new TableHeaderRow();

                    TableCell tCellHead = new TableHeaderCell();
                    tCellHead.Text = "Name";
                    tRowHead.Cells.Add(tCellHead);

                    tCellHead = new TableHeaderCell();
                    tCellHead.Text = "URL";
                    tRowHead.Cells.Add(tCellHead);

                    tCellHead = new TableHeaderCell();
                    tCellHead.Text = "Last Modified By";
                    tRowHead.Cells.Add(tCellHead);

                    ClassifableTable.Rows.Add(tRowHead);

                    foreach (Classifiable unclassified in classifiables.data)
                    {
                        TableRow tRow = new TableRow();

                        // Name
                        TableCell tCell = new TableCell();
                        tCell.Text = unclassified.name;
                        tRow.Cells.Add(tCell);

                        // Url
                        tCell = new TableCell();
                        tCell.Text = unclassified.url;
                        tRow.Cells.Add(tCell);

                        // Last Modified By
                        tCell = new TableCell();
                        tCell.Text = unclassified.classifierLastEdited.ToString();
                        tRow.Cells.Add(tCell);

                        ClassifableTable.Rows.Add(tRow);
                    }
                    LabelNotClassified.Visible = false;
                    ClassifableTable.Visible = true;
                }
                else
                {
                    LabelNotClassified.Visible = true;
                    LabelNotClassified.Text = UNCLASSFIED_NONE;
                    ClassifableTable.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GLAMClass_Exception: {0}", ex.Message);
                
                LabelNotClassified.Visible = true;
                LabelNotClassified.Text = ERROR_SERVER;
                ClassifableTable.Visible = false;
            }
        }

        /// <summary>
        /// Fetches and displays all the Classifiables that the classifier has persmission to access that
        /// are not quite classified. Such as their Concept String is incomplete.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="classifierEmail"></param>
        protected void GenerateUnclassifiedSpecial(Neo4jDB conn, string classifierEmail, Table ClassifableTable)
        {
            try
            {
                // ClassifiableCollection classifiables = conn.getAllUnclassifiedSpecial(classifierEmail);
                ClassifiableCollection classifiables = new ClassifiableCollection
                {
                    data = new List<Classifiable>(),
                };

                if (classifiables.data.Count > 0)
                {
                    // Remove the old stuff
                    ClassifableTable.Rows.Clear();

                    // Add the header row/cells
                    TableRow tRowHead = new TableHeaderRow();

                    TableCell tCellHead = new TableHeaderCell();
                    tCellHead.Text = "Name";
                    tRowHead.Cells.Add(tCellHead);

                    tCellHead = new TableHeaderCell();
                    tCellHead.Text = "Concept String";
                    tRowHead.Cells.Add(tCellHead);

                    tCellHead = new TableHeaderCell();
                    tCellHead.Text = "URL";
                    tRowHead.Cells.Add(tCellHead);

                    tCellHead = new TableHeaderCell();
                    tCellHead.Text = "Last Modified By";
                    tRowHead.Cells.Add(tCellHead);

                    ClassifableTable.Rows.Add(tRowHead);

                    foreach (Classifiable unclassified in classifiables.data)
                    {
                        TableRow tRow = new TableRow();

                        // Name
                        TableCell tCell = new TableCell();
                        tCell.Text = unclassified.name;
                        tRow.Cells.Add(tCell);

                        // Concept String
                        tCell = new TableCell();
                        //tCell.Text = unclassified.conceptStr.ToString();
                        tCell.Text = "Coming Soon!";
                        tRow.Cells.Add(tCell);

                        // Url
                        tCell = new TableCell();
                        tCell.Text = unclassified.url;
                        tRow.Cells.Add(tCell);

                        // Last Modified By
                        tCell = new TableCell();
                        //tCell.Text = unclassified.classifierLastModified.username;
                        tCell.Text = " Also Coming Soon!";
                        tRow.Cells.Add(tCell);

                        ClassifableTable.Rows.Add(tRow);
                    }

                    LabelNotClassifiedSpecial.Visible = false;
                    ClassifableTable.Visible = true;
                }
                else
                {
                    LabelNotClassifiedSpecial.Visible = true;
                    LabelNotClassifiedSpecial.Text = UNCLASSFIED_SPECIAL_NONE;
                    ClassifableTable.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GLAMClass_Exception: {0}", ex.Message);
                
                LabelNotClassifiedSpecial.Visible = true;
                LabelNotClassifiedSpecial.Text = ERROR_SERVER;
                ClassifableTable.Visible = false;
            }
        }

        /// <summary>
        /// On Click, the notification that the button corresponds to gets removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDeleteNotification_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int index = int.Parse(button.ID);

            dbConn.removeNotification(userEmail, notifications[index]);
            GenerateNotifications(dbConn, userEmail);
        }

        protected void ClassNow_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditClassifiable.aspx", true);
        }

        protected void AddNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("ClassOb.aspx", true);
        }

        protected void RemoveClassPage_Click(object sender, EventArgs e)
        {
            Response.Redirect("RemoveClassOb.aspx", true);
        }

        protected void ReClassNow_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditClassifiable.aspx", true);
        }

        protected void SuggestTerms_Click(object sender, EventArgs e)
        {
            Response.Redirect("SuggestTerm.aspx", true);
        }
    }
}