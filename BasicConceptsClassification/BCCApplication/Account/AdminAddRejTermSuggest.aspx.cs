using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Neo4j;

namespace BCCApplication.Account
{
    public partial class AdminAddRejTermSuggest : System.Web.UI.Page
    {
        protected static List<Neo4jNotification> notifications = new List<Neo4jNotification>();

        protected static char SEPARATOR_CHAR = '|';

        protected const string ERROR_PARSE = "Could not parse out individual parts. See reason.";
        protected const string ERROR_PARSE_INSTRUCTIONS = @"There was a paring error. If you wish to add the term to the BCC, 
                                                        you will have to reject this suggestion and add it manually.";
        protected const string ERROR_SERVER = "Sorry, error with the server!";

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                GetNotifications();
            }
        }

        protected void ClearFields()
        {
            txtTermName.Text = "";
            txtParentString.Text = "";
            txtMessage.Text = "";
            lblResult.Text = "";
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
                        if (tokens.Count() > 0)
                        {
                            ListBoxClass.Items.Add(tokens[0]);
                        }
                        else
                        {
                            ListBoxClass.Items.Add(note.msg);
                        }
                    }
                    ListBoxClass.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            // Try to add term
            // if term add is successful, then remove notification
            // Update Notifications List and Clear fields
            GetNotifications();
            ClearFields();
        }
        protected void btnReject_Click(object sender, EventArgs e)
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
            int selectedIndex = ListBoxClass.SelectedIndex;
            parseAndSetNotification(notifications[selectedIndex], selectedIndex);
        }
    }
}