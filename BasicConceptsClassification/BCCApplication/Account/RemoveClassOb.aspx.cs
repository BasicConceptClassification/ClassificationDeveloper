using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using BCCLib;
using Neo4j;

namespace BCCApplication.Account
{
    public partial class RemoveClassOb : System.Web.UI.Page
    {


        protected ClassifiableCollection classifiables = new ClassifiableCollection();
        protected int currentIndex = 0;

        // I don't know if we want to use status messages like this. 
        // If we do, I'm not sure how to give custom fail messages, other than making
        // multiple ones
        protected string SUCCESS = "Removed successfully.";
        protected string FAILED_GENERIC = "Unable to remove sucessfully.";
        protected string PLEASE_SELECT = "Please select a Classifiable from the list and then click the button Get Information.";

        protected void Page_Load(object sender, EventArgs e)
        {
            GetClassifiables();
        }

        protected void GetClassifiables()
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;
            var dbConn = new Neo4jDB();

            try
            {
                GLAM gl = dbConn.getGlamOfClassifier(userEmail);

                Classifier classifier = new Classifier(gl);
                classifier.name = Context.GetOwinContext().Authentication.User.Identity.Name;
                classifier.email = userEmail;

                classifiables = dbConn.getClassifiables(classifier);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ClassListBox.Visible = false;
            }

            if (classifiables.data.Count > 0)
            {
                ClassListBox.Visible = true;
                foreach (Classifiable c in classifiables.data)
                {
                    ClassListBox.Items.Add(c.name);
                }
            }
            else
            {
                ClassListBox.Visible = false;
            }    
        }

        protected int FillInClassifiableData(int index)
        {
            // Make sure we don't go out of range
            if (index < 0 || index > classifiables.data.Count) return 1;

            SelectIndex.Value = index.ToString();
            SelectName.Text = classifiables.data[index].name;
            SelectURL.Text = classifiables.data[index].url;
            SelectConceptString.Text = classifiables.data[index].conceptStr.ToString();

            System.Diagnostics.Debug.WriteLine(String.Format("RemoveClassOb_Selected classifiable: {0}", classifiables.data[index].name));

            return 0;
        }

        protected void GetClassifiableInfo_Click(object sender, EventArgs e)
        {
            currentIndex = ClassListBox.SelectedIndex;
            System.Diagnostics.Debug.WriteLine(String.Format("RemoveClassOb_Selected index: {0}", currentIndex.ToString()));
            if (FillInClassifiableData(currentIndex) != 0)
            {
                System.Diagnostics.Debug.WriteLine("RemoveClassOb_Please select a classifiable.");
                Notification.Text = PLEASE_SELECT;
            }
            else
            {
                Notification.Text = "";
            }
           
            // This feels like such a hack to:
            // a) get the index without it resetting to -1
            // b) prevent the list from doubling
            ClassListBox.Items.Clear();
            GetClassifiables();
        }

        protected void RemoveClassFromData_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("RemoveClassOb_TRYING: removing at index {0}", 
                SelectIndex.Value));



            // Try to make sure a proper index is selected
            try
            {
                Classifiable toRemove = classifiables.data[Convert.ToInt32(SelectIndex.Value)];

                // Now try to remove it
                try
                {
                    var dbConn = new Neo4jDB();
                    dbConn.deleteClassifiable(toRemove);
                    Notification.Text = SUCCESS;
                    ClearSelectFields();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    Notification.Text = FAILED_GENERIC;
                }

            }
            catch (System.FormatException)
            {
                Notification.Text = PLEASE_SELECT;
            }

            // This feels like such a hack to:
            // a) get the index without it resetting to -1
            // b) prevent the list from doubling
            ClassListBox.Items.Clear();
            GetClassifiables();
        }

        protected void ClearSelectFields()
        {
            SelectIndex.Value = "";
            SelectName.Text = "";
            SelectURL.Text = "";
            SelectConceptString.Text = "";
        }
    }
}