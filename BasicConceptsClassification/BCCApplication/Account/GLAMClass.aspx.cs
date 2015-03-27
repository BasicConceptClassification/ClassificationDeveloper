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
        private string NOTIFICATIONS_NONE = "You have no notifications.";

        private string RECENTCLASSIFIED_NONE = "No GLAM objects have been recently classified.";
        private string UNCLASSFIED_NONE = "All your GLAM objects are classified!";
        private string UNCLASSFIED_SPECIAL_NONE = "No GLAM OBjects require special attention!";
        private string ERROR_SERVER = "Having server issues, sorry!";

        static Neo4jDB dbConn = new Neo4jDB();
        static string userEmail = ""; 
        static SortedDictionary<string, string> notifications = new SortedDictionary<string, string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Manager was from register.aspx page
            // Ref: http://blogs.msdn.com/b/webdev/archive/2013/10/16/customizing-profile-information-in-asp-net-identity-in-vs-2013-templates.aspx
            // See "Getting Profile Information"
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            userEmail = currentUser.Email;

            GenerateNotifications(dbConn, userEmail);

            GenerateRecentlyClassified(dbConn, userEmail);

            GenerateUnclassified(dbConn, userEmail);

            GenerateUnclassifiedSpecial(dbConn, userEmail);
        }

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
                    foreach ( var note in notifications)
                    {
                        TableRow tRow = new TableRow();
                        TableCell tCell = new TableCell();

                        tCell.Text = note.Value;
                        tRow.Cells.Add(tCell);

                        tCell = new TableCell();

                        Button btn = new Button();
                        btn.Text = "Delete";
                        btn.ID = note.Key;
                        btn.Click += new EventHandler(btnDelete_Click);
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


        protected void GenerateRecentlyClassified(Neo4jDB conn, string classifierEmail)
        {
            try
            {
                ClassifiableCollection classifiables = conn.getRecentlyClassified(classifierEmail);
                if (classifiables.data.Count > 0)
                {
                    for (int i = 0; i < classifiables.data.Count; i++)
                    {
                        String RecClassT = classifiables.data[i].name;
                        RecClassObj.Items.Add(new ListItem(RecClassT));
                    }
                    LabelRecClassObj.Visible = false;
                    RecClassObj.Visible = true;
                }
                else
                {
                    LabelRecClassObj.Visible = true;
                    LabelRecClassObj.Text = RECENTCLASSIFIED_NONE;
                    RecClassObj.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GLAMClass_Exception: {0}", ex.Message);

                LabelRecClassObj.Visible = true;
                LabelRecClassObj.Text = ERROR_SERVER;
                RecClassObj.Visible = false;
            }
        }

        protected void GenerateUnclassified(Neo4jDB conn, string classifierEmail)
        {
            try
            {
                ClassifiableCollection classifiables = conn.getAllUnclassified(classifierEmail);
                if (classifiables.data.Count > 0)
                {
                    for (int i = 0; i < classifiables.data.Count; i++)
                    {
                        String unClassified = classifiables.data[i].name;
                        UnClassList.Items.Add(new ListItem(unClassified));
                    }
                    LabelNotClassified.Visible = false;
                    UnClassList.Visible = true;
                    ButtGLAMClassClassNow.Visible = true;
                }
                else
                {
                    LabelNotClassified.Visible = true;
                    LabelNotClassified.Text = UNCLASSFIED_NONE;
                    UnClassList.Visible = false;
                    ButtGLAMClassClassNow.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GLAMClass_Exception: {0}", ex.Message);
                
                LabelNotClassified.Visible = true;
                LabelNotClassified.Text = ERROR_SERVER;
                UnClassList.Visible = false;
                ButtGLAMClassClassNow.Visible = false;
            }
        }

        protected void GenerateUnclassifiedSpecial(Neo4jDB conn, string classifierEmail)
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
                    for (int i = 0; i < classifiables.data.Count; i++)
                    {
                        String unClassified = classifiables.data[i].name;
                        UnClassAdminCause.Items.Add(new ListItem(unClassified));
                    }
                    LabelNotClassifiedSpecial.Visible = false;
                    UnClassAdminCause.Visible = true;
                    ButtGLAMClassReClassNow.Visible = true;
                }
                else
                {
                    LabelNotClassifiedSpecial.Visible = true;
                    LabelNotClassifiedSpecial.Text = UNCLASSFIED_SPECIAL_NONE;
                    UnClassAdminCause.Visible = false;
                    ButtGLAMClassReClassNow.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GLAMClass_Exception: {0}", ex.Message);
                
                LabelNotClassifiedSpecial.Visible = true;
                LabelNotClassifiedSpecial.Text = ERROR_SERVER;
                UnClassAdminCause.Visible = false;
                ButtGLAMClassReClassNow.Visible = false;
            }
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string timestamp = button.ID;
            string message = notifications[timestamp];
            System.Diagnostics.Debug.WriteLine(timestamp);
            System.Diagnostics.Debug.WriteLine(message);

            dbConn.removeNotification(userEmail, message, timestamp);
            GenerateNotifications(dbConn, userEmail);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string str = TextBox2.Text;
            Application["textpass"] = str;
            Response.Redirect("~/SearchResults.aspx", true);
        }

        protected void ClassNow_Click(object sender, EventArgs e)
        {
            Response.Redirect("ClassOb.aspx", true);
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
            Response.Redirect("ClassOb.aspx", true);
        }
    }
}