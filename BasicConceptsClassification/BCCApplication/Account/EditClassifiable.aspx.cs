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
            //ListBox2.Items.Clear();
            //ListBoxClass.Items.Clear();
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





            ClassifiableCollection classifieds = dbConn.getAllClassified(userEmail);
            int resultLength = classifieds.data.Count;
            for (int i = 0; i < resultLength; i++)
            {
                Classifiable currentClassifiable = classifieds.data[i];
                ListBoxClass.Items.Add(currentClassifiable.name);
               // ListBox4.Items.Add("-------------------------------------");
                //ListBox4.Items.Add(classifieds.data[i].id);
                //ListBox4.Items.Add(classifieds.data[i].name);
                //ListBox4.Items.Add(classifieds.data[i].owner.ToString());
                //ListBox4.Items.Add(classifieds.data[i].perm);
                //ListBox4.Items.Add(classifieds.data[i].status);
                //classifieds.data[i].conceptStr
               // List<string> TERMS = classifieds.data[i].conceptStr.ToListstring();
               // foreach (string things in TERMS)
                //{
                //    ListBox4.Items.Add(things);
               // }
                //ListBox4.Items.Add(classifieds.data[i].conceptStr.ToString());
                //ListBox4.Items.Add("-------------------------------------");
            }

            
            
        }

       

        protected void Edit_Click(object sender, EventArgs e)
        {
            string str_n = TextBox_Name.Text;
            string str_u = TextBox_URL.Text;
            string str_c = TextBox_Concept.Text;




            //-----------------------------set the new classifiable------------------------------------
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;

            // TODO: cleanup. It's a bit...messy.
            var conn = new Neo4jDB();
            GLAM gl = conn.getGlamOfClassifier(userEmail);

            Classifier classifier = new Classifier(gl);
            classifier.username = Context.GetOwinContext().Authentication.User.Identity.Name;
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
            string the_status;
            if (str_c != "")
            {
                the_status = Classifiable.Status.Classified.ToString();
            }
            else{
                the_status = Classifiable.Status.Unclassified.ToString();
            }
            Classifiable newClassifiable = new Classifiable
            {
                id = gl.name + "_" + str_n,
                name = str_n,
                url = str_u,
                perm = Classifiable.Permission.GLAM.ToString(),
                status = the_status,
                owner = classifier,
                conceptStr = newConceptStr,
            };


            //ListBox3.Items.Add(newClassifiable.id);
            //ListBox3.Items.Add(newClassifiable.name);
            //ListBox3.Items.Add(newClassifiable.owner.ToString());
            //ListBox3.Items.Add(newClassifiable.perm);
           // ListBox3.Items.Add(newClassifiable.status);
            //ListBox3.Items.Add(newClassifiable.conceptStr.ToString());

            //List<string> TERMS = newClassifiable.conceptStr.ToListstring();
            //foreach (string things in TERMS)
            //{
            //    ListBox3.Items.Add(things);
           // }
            

            //-------------------------------------------------------------------------------------------


           // var dbConn = new Neo4jDB();


            Classifiable matchedClassifiable = conn.getClassifiableById(gl.name+ "_" + select_string);

            string teststr = "";

            try
            {
                teststr = matchedClassifiable.conceptStr.ToString();
            }
            catch
            {

            }

            
            conn.updateClassifiable(matchedClassifiable, newClassifiable, classifier);
            

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
            classifier.username = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = userEmail;


            Classifiable matchedClassifiable = dbConn.getClassifiableById(gl.name+ "_" + select_string);
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
            classifier.username = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = userEmail;

            Classifiable matchedClassifiable = dbConn.getClassifiableById(gl.name+ "_" + select_string);
            TextBox_Name.Text = select_string;
            TextBox_URL.Text = matchedClassifiable.url;
            TextBox_Concept.Text = matchedClassifiable.conceptStr.ToString();
        }




    }
}