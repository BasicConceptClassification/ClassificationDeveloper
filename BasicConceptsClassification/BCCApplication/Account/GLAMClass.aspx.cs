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
        private string RECENTCLASSIFIED_NONE = "No GLAM objects have been recently classified.";
        private string UNCLASSFIED_NONE = "All your GLAM objects are classified!";
        private string UNCLASSFIED_SPECIAL_NONE = "No GLAM OBjects require special attention!";
        private string ERROR_SERVER = "Having server issues, sorry!";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Manager was from register.aspx page
            // Ref: http://blogs.msdn.com/b/webdev/archive/2013/10/16/customizing-profile-information-in-asp-net-identity-in-vs-2013-templates.aspx
            // See "Getting Profile Information"
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;

            var dbConn = new Neo4jDB();

            GenerateTermUpdates();

            GenerateRecentlyClassified(dbConn, userEmail);

            GenerateUnclassified(dbConn, userEmail);

            GenerateUnclassifiedSpecial(dbConn, userEmail);
        }

        protected void GenerateTermUpdates()
        {
            // TODO: Need to turn this into a notification system?
            try
            {
                // Dealing with list of recently added terms hooked up to respective Classifier
                int numRecAdd = 7;     //have read number of unclassified objects
                for (int i = 0; i < numRecAdd; i++)
                {
                    String RecAddT = "New Term " + i + 1;          // change to read actual recently added string
                    RecAddedTerms.Items.Add(new ListItem(RecAddT));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GLAMClass_Exception: {0}", ex.Message);
                // Some sort of notification?
                String RecAddT = "Sorry, server is having issues!";
                RecAddedTerms.Items.Add(new ListItem(RecAddT));          
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