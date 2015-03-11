using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCCLib;
using Neo4j;
using System.Web.Security;

namespace BCCApplication.Account
{
    public partial class ClassOb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SubmitObj_Click(object sender, EventArgs e)
        {
            string t_url = ObURL.Text;
            string t_name = ObName.Text;
            string t_concept = ObConcept.Text;
            var conn = new Neo4jDB();

            GLAM gl = new GLAM("UA", "www.ualberta.ca");
            Classifier class_fier = new Classifier(gl);
            class_fier.email = "www@ualberta.ca";





            //Classifiable adding_classifiable = new Classifiable();

            //adding_classifiable.name = name;

            //ConceptString add_concept = new ConceptString();

            string Triminput_str = t_concept.Trim();
            string sstring = Triminput_str.Replace(")(", ",");
            sstring = sstring.Replace(")", "");
            sstring = sstring.Replace("(", "");
            List<string> new_str = sstring.Split(',').ToList();
           // List<string> result_needs = new List<string>();
          
            // foreach (string things in new_str)
           // {

           //     result_needs.Add("(" + things + ")");
           // }

          //  foreach (string things in result_needs)
          //  {
          //      ListBox1.Items.Add(things);
          //  }

            List<Term> new_terms = new List<Term>();

            foreach (String things in new_str)
            {
                //change to terms
                Term terterma = new Term { rawTerm = things, };
                //ListBox1.Items.Add(things);
                new_terms.Add(terterma);
            }

            ConceptString add_concept = new ConceptString
            {
                terms = new_terms,

            };
            //--------------------------------------------------------------

            /*
            string test_name = Membership.GetUser(User.Identity.Name).UserName;
            string test_email = Membership.GetUser(User.Identity.Name).Email;
            ListBox1.Items.Add(test_email);
            ListBox1.Items.Add(test_name);
            */


            class_fier.name = "Tony";//Membership.GetUser(User.Identity.Name).UserName;
            class_fier.email ="forfun@ualberta.ca";// Membership.GetUser(User.Identity.Name).Email;


            Classifiable newClassifiable = new Classifiable
            {
                id = gl.name + "_" + "dummyName1",
                name = t_name,
                url = t_url,
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = class_fier,
                conceptStr = add_concept,
            };
            //adding_classifiable.id = "10001";
           // adding_classifiable.name = name;
            //adding_classifiable.conceptStr = add_concept;
           // adding_classifiable.url = url;
           // adding_classifiable.perm = "None";
           // adding_classifiable.status = "classified";
           // adding_classifiable.owner = class_fier;
            /*
            ListBox1.Items.Add(adding_classifiable.id);
            ListBox1.Items.Add(adding_classifiable.name);
            //ListBox1.Items.Add(adding_classifiable.conceptStr.ToString);
            //List<string> tttest = adding_classifiable.conceptStr.T;
            ListBox1.Items.Add(adding_classifiable.conceptStr.ToString());
            ListBox1.Items.Add(adding_classifiable.perm);
            ListBox1.Items.Add(adding_classifiable.status);
            ListBox1.Items.Add(adding_classifiable.owner.name);
            ListBox1.Items.Add(adding_classifiable.id);
             */


            Classifiable result = conn.addClassifiable(newClassifiable);

            string searchById = "10001";

            Classifiable classifiedWithGoodId = conn.getClassifiableById(searchById);
            //ListBox1.Items.Add(conn.countNumTermsExist(adding_classifiable.conceptStr.terms));
            //ListBox1.Items.Add(classifiedWithGoodId.id);
            //ListBox1.Items.Add(classifiedWithGoodId.name);
            //ListBox1.Items.Add(adding_classifiable.conceptStr.ToString);
            //List<string> tttest = adding_classifiable.conceptStr.T;
            ListBox1.Items.Add(classifiedWithGoodId.conceptStr.ToString());
            ListBox1.Items.Add(classifiedWithGoodId.perm);
            ListBox1.Items.Add(classifiedWithGoodId.status);
            ListBox1.Items.Add(classifiedWithGoodId.owner.name);
            ListBox1.Items.Add(classifiedWithGoodId.id);
            //adding_classifiable.owner = "Temp";


            //adding_classifiable.conceptStr = concept;
            conn.deleteClassifiable(result);
            conn.deleteClassifier(class_fier);
           
        }


    }
}