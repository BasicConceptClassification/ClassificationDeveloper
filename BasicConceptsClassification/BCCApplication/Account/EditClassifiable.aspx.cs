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
        static string select_string;

        protected void Page_Load(object sender, EventArgs e)
        {
            ListBox2.Items.Clear();
            ListBoxClass.Items.Clear();
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
                ListBox2.Items.Add(currentUNClassifiable.id);
            }
            ListBox2.Items.Add("things");




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



            //-----------------------------set the new classifiable------------------------------------
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;

            // TODO: cleanup. It's a bit...messy.
            var conn = new Neo4jDB();
            GLAM gl = conn.getGlamOfClassifier(userEmail);

            Classifier classifier = new Classifier(gl);
            classifier.name = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = userEmail;

            // TODO: either make a constructor for ConceptString to take (this)(format) and have it parse
            // it out so we don't have to see this parsing every single time AND create terms from it?
            //split the input concept string from (xx)(xx)(xx) to a list without () 
            string trimConceptString = str_c.Trim();

            //convert the string list to the term list
            List<Term> newTerms = new List<Term>();

            // Only extract terms if there are any terms added
            if (trimConceptString != "")
            {
                string sstring = trimConceptString.Replace(")(", ",");
                sstring = sstring.Replace(")", "");
                sstring = sstring.Replace("(", "");
                //new_str is the result list 
                List<string> newStr = sstring.Split(',').ToList();

                foreach (String termStr in newStr)
                {
                    //change to terms
                    Term terterma = new Term { rawTerm = termStr, };
                    newTerms.Add(terterma);
                }
            }

            ConceptString newConceptStr = new ConceptString
            {
                terms = newTerms,
            };


            Classifiable newClassifiable = new Classifiable
            {
                id = classifier.name + "_" + str_n,
                name = str_n,
                url = str_u,
                perm = EditPerm.SelectedValue,
                owner = classifier,
                conceptStr = newConceptStr,
            };
            //-------------------------------------------------------------------------------------------


           // var dbConn = new Neo4jDB();


            Classifiable matchedClassifiable = conn.getClassifiableById(/*classifier.name +*/ "_" + select_string);

            string teststr = "";

            try
            {
                teststr = matchedClassifiable.conceptStr.ToString();
            }
            catch
            {

            }

            if (teststr != "")
            {
                conn.updateClassifiable(matchedClassifiable, newClassifiable, classifier);
            }
            else
            {
                
            }

        }



        protected void Update_Class_Click(object sender, EventArgs e)
        {
            select_string = "";
            select_string = ListBoxClass.SelectedItem.ToString();
            TextBox_Name.Text = select_string;
            var dbConn = new Neo4jDB();

            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;

            // TODO: cleanup. It's a bit...messy.
            var conn = new Neo4jDB();
            GLAM gl = conn.getGlamOfClassifier(userEmail);

            Classifier classifier = new Classifier(gl);
            classifier.name = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = userEmail;


            Classifiable matchedClassifiable = dbConn.getClassifiableById(/*classifier.name +*/ "_" + select_string);
            TextBox_Name.Text = matchedClassifiable.name;
            TextBox_URL.Text = matchedClassifiable.url;
            TextBox_Concept.Text = matchedClassifiable.conceptStr.ToString();
        }

        protected void Update_Unclass_Click(object sender, EventArgs e)
        {
            select_string = "";
            //var selected = list
            select_string = ListBox2.SelectedItem.ToString();
            //TextBox_Name.Text = select_string;
            var dbConn = new Neo4jDB();

            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;

            // TODO: cleanup. It's a bit...messy.
            var conn = new Neo4jDB();
            GLAM gl = conn.getGlamOfClassifier(userEmail);

            Classifier classifier = new Classifier(gl);
            classifier.name = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = userEmail;

            Classifiable matchedClassifiable = dbConn.getClassifiableById(/*classifier.name*/ "_" + select_string);
            TextBox_Name.Text = classifier.name + "_" + select_string;
            TextBox_URL.Text = matchedClassifiable.url;
            TextBox_Concept.Text = matchedClassifiable.conceptStr.ToString();
        }




    }
}