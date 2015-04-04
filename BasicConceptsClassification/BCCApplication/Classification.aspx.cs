using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BCCLib;
using Neo4j;

namespace BCCApplication
{
    public partial class Classification : System.Web.UI.Page
    {
        private string DESCRIPTION_1 = @"<p>Learn all about the Classification on this page!</p>
                                        <p>Galleries, Libraries, Archives and Museums (GLAMs) around the world have databases that are ineffective to search
                                        through due to isolated systems and a non-uniform vocabulary. <b><i>Basic Concepts Classification</i></b> hopes
                                        to address both these problems with an easy to learn platform for classification and searching of the collections
                                        of GLAMs around the world.</p>";

        private string DESCRIPTION_2 = "<p>Details of the BCC can be found on the <a href=\"/Search\">Search Aid Page</a>.</p>";

        private string DESCRIPTION_3 = @"<p>With this system, curators from different institutions will be able to add items to a centralized database, and all
                                        users will be able to search through this database.</p>";                     

        private string DESC_CLASSIFIED_EX = "<h3>Here are some examples of GLAM objects classified using the BCC</h3>";

        private string EXAMPLE_CLASSIFIABLE = "Adze Blade";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Load the general description
            LabelDescription.Text = DESCRIPTION_1 + DESCRIPTION_2 + DESCRIPTION_3;

            // Load some examples
            LabelClassifiedExamples.Text = DESC_CLASSIFIED_EX;

            ClassifiableCollection examples = new ClassifiableCollection { data = new List<Classifiable>() };
            try
            {
                var conn = new Neo4jDB();

                examples = conn.getClassifiablesByName(EXAMPLE_CLASSIFIABLE);
            }
            catch (Exception ex)
            { 
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            if (examples.data.Count > 0)
            {
                string classifiableExample = String.Format("{0} is classified by: {1}",
                    examples.data[0].name, examples.data[0].conceptStr.ToString());
                BulletLExamples.Items.Add(classifiableExample);
            }
            else 
            {
                BulletLExamples.Items.Add(String.Format("Server could not find the GLAM Object called {0}.", EXAMPLE_CLASSIFIABLE));
            }
        }
    }
}