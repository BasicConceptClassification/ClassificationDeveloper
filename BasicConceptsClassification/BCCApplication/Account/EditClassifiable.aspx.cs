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
    public partial class EditClassifiable : System.Web.UI.Page
    {
        string select_string;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            var dbConn = new Neo4jDB();
            // Get the logged in user's email
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;



            ClassifiableCollection unclassifieds = dbConn.getAllUnclassified(userEmail);
            int resultsLength = unclassifieds.data.Count;
            for (int i = 0; i < resultsLength; i++)
            {
                Classifiable currentUNClassifiable = unclassifieds.data[i];
                ListBox2.Items.Add(currentUNClassifiable.name);
            }




            ClassifiableCollection classifieds = dbConn.getRecentlyClassified(userEmail);
            int resultLength = unclassifieds.data.Count;
            for (int i = 0; i < resultLength; i++)
            {
                Classifiable currentClassifiable = classifieds.data[i];
                ListBoxClass.Items.Add(currentClassifiable.name);
            }

        }

       

        protected void Edit_Click(object sender, EventArgs e)
        {
            string str_n = TextBox_Name.Text;
            string str_u = TextBox_URL.Text;
            string str_c = TextBox_Concept.Text;
            //string str_perm = EditPerm.SelectedValue;
            //string str_owner = classifier;
            Application["namepass"] = str_n;
            Application["urlepass"] = str_u;
            Application["conpass"] = str_c;
            Response.Redirect("ClassOb.aspx", true);

        }



        protected void Update_Class_Click(object sender, EventArgs e)
        {
            select_string = ListBoxClass.SelectedItem.ToString();
            //TextBox_Name.Text = select_string;
            var dbConn = new Neo4jDB();
            Classifiable matchedClassifiable = dbConn.getClassifiableById(select_string);
            TextBox_Name.Text = matchedClassifiable.name;
            TextBox_URL.Text = matchedClassifiable.url;
            TextBox_Concept.Text = matchedClassifiable.conceptStr.ToString();
        }

        protected void Update_Unclass_Click(object sender, EventArgs e)
        {
            select_string = ListBoxClass.SelectedItem.ToString();
            //TextBox_Name.Text = select_string;
            var dbConn = new Neo4jDB();
            Classifiable matchedClassifiable = dbConn.getClassifiableById(select_string);
            TextBox_Name.Text = matchedClassifiable.name;
            TextBox_URL.Text = matchedClassifiable.url;
            TextBox_Concept.Text = matchedClassifiable.conceptStr.ToString();
        }




    }
}