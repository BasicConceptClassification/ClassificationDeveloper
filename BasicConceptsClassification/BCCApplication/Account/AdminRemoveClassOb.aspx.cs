using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BCCLib;
using Neo4j;

namespace BCCApplication.Account
{
    public partial class AdminRemoveClassOb : System.Web.UI.Page
    {
        private ClassifiableCollection alphaClassCollection = new ClassifiableCollection
        {
            data = new List<Classifiable>(),
        };

        private String ALPHABET = "-ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private int selectedLetterIndex = 0;

        private String CLASSIFIABLES_EXIST = "Classifiables";
        private String CLASSIFIABLES_NONE = "No classifiables begin with that letter";
        private String ERROR_SERVER = "Sorry, there's an issue with the server!";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                foreach (char letter in ALPHABET)
                {
                    AlphabetDDL.Items.Add(letter.ToString());
                }
            }
        }

        protected void AlphabetDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(ALPHABET[AlphabetDDL.SelectedIndex]);

            selectedLetterIndex = AlphabetDDL.SelectedIndex;


            // 0th index is the char '-', so don't do anything.
            if (selectedLetterIndex > 0)
            {
                try
                {
                    var dbConn = new Neo4jDB();
                    alphaClassCollection = dbConn.getClassifiablesByAlphaGroup(ALPHABET[selectedLetterIndex]);

                    System.Diagnostics.Debug.WriteLine(alphaClassCollection.data.Count);

                    if (alphaClassCollection.data.Count > 0)
                    {
                        // Make the listbox of classifiables visible and populate it
                        LabelClassListBox.Text = CLASSIFIABLES_EXIST;
                        ClassListBox.Visible = true;
                        ClassListBox.Items.Clear();

                        System.Diagnostics.Debug.WriteLine(alphaClassCollection.data[0].name);
                        foreach (Classifiable c in alphaClassCollection.data)
                        {

                            ClassListBox.Items.Add(c.name);
                        }
                    }
                    else
                    {
                        // HIde the Classifiables listbox and show a message explaining
                        // that there are none
                        LabelClassListBox.Text = CLASSIFIABLES_NONE;
                        ClassListBox.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    LabelClassListBox.Text = ERROR_SERVER;
                    ClassListBox.Visible = false;
                }
            }
        }

        protected void ClassListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void BtnRemove_Click(object sender, EventArgs e)
        {

        }
    }
}