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

            // create a temp GALM for testing
            // TODO: fetch proper GLAM
            GLAM gl = new GLAM("UA");

            // TODO: Fetch email properly
            Classifier classifier = new Classifier(gl);
            classifier.name = Context.GetOwinContext().Authentication.User.Identity.Name;
            classifier.email = "somewhere@com";

            // TODO: either make a constructor for ConceptString to take (this)(format) and have it parse
            // it out so we don't have to see this parsing every single time AND create terms from it?
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

            // TODO: GET PROPER VALUES:
            // id - created by "<GLAM_NAME>_<CLASSIFIABLE_NAME>
            // perm - get from the radio button!
            // status - look at ConStr and if == "" then it's Unclassified,
            // or tell Bronte to make addClassifiable deal with it...
            Classifiable newClassifiable = new Classifiable
            {
                id = classifier.name + "_" + input_name,
                name = input_name,
                url = input_url,
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = add_concept,
            };

            Classifiable result = new Classifiable();

            var conn = new Neo4jDB();

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
                    //     // do something about cannot commit transaction...
                    // }

                    // try
                    // {
                    // conn.deleteClassifier(classifier);
                    // }
                    // catch (Exception ex)
                    // {
                    //     // do something about cannot commit transaction...
                    // }
                }

            }
            catch (Exception ex)
            {
                // Exceptions: Unique id already exists, null object (not all data filled in)
                // Do some exception handling based on Exception type ...learn how to do custom exceptions?
                Debug.WriteLine(ex.Message);
            }           
        }
    }
}