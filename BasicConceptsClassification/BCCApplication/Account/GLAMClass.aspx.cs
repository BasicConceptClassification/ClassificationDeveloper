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
    public partial class GLAMClass : System.Web.UI.Page
    {
        private string DESCRIPTION = @"<p>Below you can see your notifications, your recently classified GLAM objects, those not classified,
                                        or those that might need some special attention.
                                        You can also Add new GLAM objects to your GLAM or remove any GLAM objects that you have added.</p>";

        private string NOTIFICATIONS_NONE = "You have no notifications.";

        private string RECENTCLASSIFIED_NONE = "No GLAM objects have been recently classified.";
        private string UNCLASSFIED_NONE = "All your GLAM objects are classified!";
        private string UNCLASSFIED_SPECIAL_NONE = "No GLAM OBjects require special attention!";
        private string ERROR_SERVER = "Having server issues, sorry!";

        static Neo4jDB dbConn = new Neo4jDB();
        static string userEmail = ""; 
        static List<Neo4jNotification> notifications = new List<Neo4jNotification>();
        static ClassifiableCollection recentClassifiables = new ClassifiableCollection
        {
            data = new List<Classifiable>(),
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            // Manager was from register.aspx page
            // Ref: http://blogs.msdn.com/b/webdev/archive/2013/10/16/customizing-profile-information-in-asp-net-identity-in-vs-2013-templates.aspx
            // See "Getting Profile Information"
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            userEmail = currentUser.Email;

            LabelDescription.Text = DESCRIPTION;
       
                GenerateNotifications(dbConn, userEmail);

                GenerateRecentlyClassified(dbConn, userEmail);

                GenerateUnclassified(dbConn, userEmail);

                GenerateUnclassifiedSpecial(dbConn, userEmail);
            
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
        protected void GenerateRecentlyClassified(Neo4jDB conn, string classifierEmail)
        {
            try
            {
                recentClassifiables = conn.getRecentlyClassified(classifierEmail);
                
                if (recentClassifiables.data.Count > 0)
                {
                    RecClassObj.Items.Clear();
                    for (int i = 0; i < recentClassifiables.data.Count; i++)
                    {
                        String RecClassT = recentClassifiables.data[i].name;
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

        /// <summary>
        /// Fetches and fills in the unclassified classifiables that the Classifier has persmission to access.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="classifierEmail"></param>
        protected void GenerateUnclassified(Neo4jDB conn, string classifierEmail)
        {
            try
            {
                ClassifiableCollection classifiables = conn.getAllUnclassified(classifierEmail);
                if (classifiables.data.Count > 0)
                {
                    for (int i = 0; i < classifiables.data.Count; i++)
                    {
                        UnClassList.Items.Clear();
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

        /// <summary>
        /// Fetches and displays all the Classifiables that the classifier has persmission to access that
        /// are not quite classified. Such as their Concept String is incomplete.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="classifierEmail"></param>
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
                    UnClassAdminCause.Items.Clear();
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
    }
}