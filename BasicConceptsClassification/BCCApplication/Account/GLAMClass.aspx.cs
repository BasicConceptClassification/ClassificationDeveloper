using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BCCApplication.Account
{
    public partial class GLAMClass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Dealing with list of recently added terms hooked up to respective Classifier
            int numRecAdd = 7;     //have read number of unclassified objects
            for (int i = 0; i < numRecAdd; i++)
            {
                String RecAddT = "New Term " + i+1;          // change to read actual recently added string
                RecAddedTerms.Items.Add(new ListItem(RecAddT));
            }


            // Dealing with list of recently classified terms hooked up to respective Classifier
            int numRecClass = 7;     //have read number of unclassified objects
            for (int i = 0; i < numRecClass; i++)
            {
                String RecClassT = "Recently Classified Term " + i+1;          // change to read actual recently classified string
                RecClassTerms.Items.Add(new ListItem(RecClassT));
            }


            // Dealing with list of unclassified objects hooked up to respective Classifier
            int numUnClass = 7;     //have read number of unclassified objects
            for (int i = 0; i < numUnClass; i++) {
                String unClassified = "Unclassified " + i+1;          // change to read actual unclassified string
                UnClassList.Items.Add(new ListItem(unClassified));
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string str = TextBox2.Text;
            Application["textpass"] = str;
            Server.Transfer("SearchResults.aspx", true);
        }

        protected void ClassNow_Click(object sender, EventArgs e)
        {
            Server.Transfer("ClassOb.aspx", true);
        }

        protected void AddNew_Click(object sender, EventArgs e)
        {
            Server.Transfer("ClassOb.aspx", true);
        }
    }
}