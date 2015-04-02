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
        private string SUCCESS = "Successfully Edite the classifiable. ";
        private string FAIL = "Fail to Edite the classifiable. ";


        /// <summary>
        /// when the page load it display the classified and unclassified elements on two lists.
        /// </summary>
        /// <return> nothing will return </returns>
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
                ListBox2.Items.Add(currentUNClassifiable.id);
            }

            ClassifiableCollection classifieds = dbConn.getAllClassified(userEmail);
            int resultLength = classifieds.data.Count;
            for (int i = 0; i < resultLength; i++)
            {
                Classifiable currentClassifiable = classifieds.data[i];
                ListBoxClass.Items.Add(currentClassifiable.name);
            }
        
        }


        /// <summary>
        /// click the Edit button and to edite the current classified or unclassified elements
        /// </summary>
        /// <return> nothing will return </returns>
        protected void Edit_Click(object sender, EventArgs e)
        {
            string str_n = TextBox_Name.Text;
            string str_u = TextBox_URL.Text;
            string str_c = TextBox_Concept.Text;

            //-----------------------------set the new classifiable------------------------------------
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;

            var conn = new Neo4jDB();
            GLAM gl = conn.getGlamOfClassifier(userEmail);

            Classifier classifier = new Classifier(gl);
            classifier.username = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = userEmail;

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

            //setting the new classifiable
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

            Classifiable matchedClassifiable = conn.getClassifiableById(gl.name+ "_" + select_string);

            //check is there have any concept string
            string teststr = "";
            
            try
            {
                teststr = matchedClassifiable.conceptStr.ToString();
            }
            catch
            {

            }

            //update the edited classifiable
            //and return the message to user.
            try
            {
                conn.updateClassifiable(matchedClassifiable, newClassifiable, classifier);
                Label1.Text = SUCCESS;
            }
            catch
            {
                Label1.Text = FAIL;
            }
        }


        /// <summary>
        /// click the update button to pop the choosen elment's detials onto textbox
        /// </summary>
        /// <return> nothing will return </returns>
        protected void Update_Class_Click(object sender, EventArgs e)
        {
            select_string = "";
            select_string = ListBoxClass.SelectedItem.ToString();
            TextBox_Name.Text = select_string;
            var dbConn = new Neo4jDB();

            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string userEmail = currentUser.Email;

            var conn = new Neo4jDB();
            GLAM gl = conn.getGlamOfClassifier(userEmail);

            Classifier classifier = new Classifier(gl);
            classifier.username = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = userEmail;

            //and return the message to user.
            try
            {
                Classifiable matchedClassifiable = dbConn.getClassifiableById(gl.name + "_" + select_string);
                TextBox_Name.Text = matchedClassifiable.name;
                TextBox_URL.Text = matchedClassifiable.url;
                TextBox_Concept.Text = matchedClassifiable.conceptStr.ToString();
            }
            catch
            {
                Label1.Text = "Please choose one of the classifiable element in the list.";
            }
            
        }

        /// <summary>
        /// click the update button to pop the choosen elment's detials onto textbox
        /// </summary>
        /// <return> nothing will return </returns>
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

            var conn = new Neo4jDB();
            GLAM gl = conn.getGlamOfClassifier(userEmail);

            Classifier classifier = new Classifier(gl);
            classifier.username = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = userEmail;

            //and return the message to user.
            try
            {
                Classifiable matchedClassifiable = dbConn.getClassifiableById(gl.name + "_" + select_string);
                TextBox_Name.Text = select_string;
                TextBox_URL.Text = matchedClassifiable.url;
                TextBox_Concept.Text = matchedClassifiable.conceptStr.ToString();
            }
            catch
            {
                Label1.Text = "Please choose one of the unclassifiable element in the list.";
            }
            
        }

    }
}