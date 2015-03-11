using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCCLib;
using Neo4j;
using System.Web.Security;

using System.Diagnostics;

namespace BCCApplication.Account
{
    public partial class ClassOb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //the control submit button
        protected void SubmitObj_Click(object sender, EventArgs e)
        {
            //I create a listbox for testing or watching the results.
            //which is listbox1, and you can use by calling     listbox1.Items.Add(things that you want to print out)

            //input of each text box
            //get vaule of each
            string input_url = ObURL.Text;
            string input_name = ObName.Text;
            string input_concept = ObConcept.Text;

            // DEBUGGING
            Debug.WriteLine(String.Format("Name: {0}, ConceptString: {1}, URL: {2}", 
                input_name, input_concept, input_url));

            var conn = new Neo4jDB();

            // create a temp GALM for testing
            // TODO: fetch proper GLAM
            GLAM gl = new GLAM("UA", "www.ualberta.ca");

            // TODO: Fetch email properly
            Classifier classifier = new Classifier(gl);
            classifier.name = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = "somewhere@com";

            // DEBUG
            Debug.WriteLine(String.Format("Classifier name: {0}, Classifier email: {1},", 
                classifier.name, classifier.email));

            // TODO: either make a constructor for ConceptString to take (this)(format) and have it parse
            // it out so we don't have to see this parsing every single time...
            //split the input concept string from (xx)(xx)(xx) to a list without () 
            string Triminput_str = input_concept.Trim();
            string sstring = Triminput_str.Replace(")(", ",");
            sstring = sstring.Replace(")", "");
            sstring = sstring.Replace("(", "");
            //new_str is the result list 
            List<string> new_str = sstring.Split(',').ToList();

            //convert the string list to the term list
            List<Term> new_terms = new List<Term>();

            foreach (String things in new_str)
            {
                //change to terms
                Term terterma = new Term { rawTerm = things, };
                new_terms.Add(terterma);
            }

            ConceptString add_concept = new ConceptString
            {
                terms = new_terms,
            };

            // DEBUG
            Debug.WriteLine(String.Format("ConStr extracted out: {0}", add_concept.ToString()));

            // TODO: FETCH PROPER VALUES FORM WEBPAGE
            Classifiable newClassifiable = new Classifiable
            {
                id = "10001",
                name = input_name,
                url = input_url,
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = add_concept,
            };

            Classifiable result = new Classifiable();

            try
            {
                result = conn.addClassifiable(newClassifiable);

                if (result != null)
                {
                    //delete the testing stuff
                    // try
                    // {
                    // conn.deleteClassifiable(result);
                    // }
                    // catch (Exception ex)
                    // {
                    //     // do something 
                    // }

                    // try
                    // {
                    // conn.deleteClassifier(classifier);
                    // }
                    // catch (Exception ex)
                    // {
                    //     // do something 
                    // }
                }

            }
            catch (Exception ex)
            {
                // Do some exception handling based on Exception type ...learn how to do custom exceptions?
                Debug.WriteLine(ex.Message);
            }           
        }
    }
}