using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BCCLib;
using Neo4j;
using System.Diagnostics;

namespace BCCApplication.Account
{
    public partial class GLAMClass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var dbConn = new Neo4jDB();

            // Realized I still don't know how to fetch Glam and classifier somehow...
            GLAM g = new GLAM("Not A Real GLAM");

            Classifier currentClassifier = new Classifier(g);
            currentClassifier.email = "doesnotexist@someplace.com";

            // Comment out the above and uncomment below for how it will look 
            // for an example
            /*
            GLAM g = new GLAM("National Gallery (US)");

            Classifier currentClassifier = new Classifier(g);
            currentClassifier.email = "user2@NationalGalleryUS.com";
            */
            
            GenerateTermUpdates();

            GenerateRecentlyClassified(dbConn, currentClassifier.email);

            GenerateUnclassified(dbConn, currentClassifier.email);
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
            catch
            {
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
                for (int i = 0; i < classifiables.data.Count; i++)
                {
                    String RecClassT = classifiables.data[i].name;
                    RecClassTerms.Items.Add(new ListItem(RecClassT));
                }
            }
            catch
            {
                // Some sort of notification?
                String RecClassT = "Sorry, server is having issues!";
                RecClassTerms.Items.Add(new ListItem(RecClassT));
            }

        }

        protected void GenerateUnclassified(Neo4jDB conn, string classifierEmail)
        {
            try
            {
                ClassifiableCollection classifiables = conn.getAllUnclassified(classifierEmail);
                for (int i = 0; i < classifiables.data.Count; i++)
                {
                    String unClassified = classifiables.data[i].name;
                    UnClassList.Items.Add(new ListItem(unClassified));
                }
            }
            catch
            {
                // Some sort of notification?
                String unClassified = "Sorry, server is having issues!"; 
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