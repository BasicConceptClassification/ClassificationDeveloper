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

        // Examples that the client wanted. Could be modifiable by edit content?
        private List<string> EXAMPLE_CLASSIFIABLE = new List<string> 
        { 
            "Hamons Court [Neon Sign]",
            "Towne of Boston",
            "16-Pound Bar Shot",
            "Clephane horn",
            "Venus with a mirror",
            "Pan reclining",
            "The Maas at Dordrecht",
            "Woman holding a balance",
        };

        // Contains the classifiables to show as an example
        private ClassifiableCollection exampleClassifiables = new ClassifiableCollection { data = new List<Classifiable>(), };

        protected void Page_Load(object sender, EventArgs e)
        {
            // Load the general description
            LabelDescription.Text = DESCRIPTION_1 + DESCRIPTION_2 + DESCRIPTION_3;

            // Load some examples
            LabelClassifiedExamples.Text = DESC_CLASSIFIED_EX;
            
            var conn = new Neo4jDB();

            foreach (var classifiableName in EXAMPLE_CLASSIFIABLE)
            {
                Classifiable tmp = null;

                // Try to get some results. If some have the same name, multiples can be returned. Only grab the first
                try
                {
                    ClassifiableCollection tmpColl = conn.getClassifiablesByName(classifiableName);
                    if (tmpColl.data.Count > 0) tmp = tmpColl.data[0];
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                // Only add if we got a result. Otherwise it's just skipped.
                if (tmp != null)
                {
                    exampleClassifiables.data.Add(tmp);
                }
            }

            // If we find any results, display them. If none are found, then display an error message.
            if (exampleClassifiables.data.Count > 0)
            {
                foreach (var GLAMObj in exampleClassifiables.data)
                {
                    string classifiableExample = String.Format("{0} is classified by: {1}",
                        GLAMObj.name, GLAMObj.conceptStr.ToString());
                    BulletLExamples.Items.Add(classifiableExample);
                }
            }
            else 
            {
                BulletLExamples.Items.Add("Server could not find any of the example GLAM Objects.");
            }
        }
    }
}