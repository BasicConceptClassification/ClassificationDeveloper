using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCCLib;
using Neo4j;

namespace BCCApplication.Account
{
    public partial class EditClassifiable : System.Web.UI.Page
    {
        string select_string;

        protected void Page_Load(object sender, EventArgs e)
        {
            ListBoxClass.Items.Add("AAAA");
            ListBoxClass.Items.Add("BBBB");
            ListBoxClass.Items.Add("CCCC");
            ListBoxClass.Items.Add("DDDD");
            ListBoxClass.Items.Add("EEEE");
            ListBoxClass.Items.Add("FFFF");


        }

       

        protected void Edit_Click(object sender, EventArgs e)
        {
            string str_n = TextBox_Name.Text;
            string str_u = TextBox_URL.Text;
            string str_c = TextBox_Concept.Text;
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