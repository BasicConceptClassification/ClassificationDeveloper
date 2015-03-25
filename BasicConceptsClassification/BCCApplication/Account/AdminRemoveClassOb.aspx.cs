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
        private static ClassifiableCollection alphaClassCollection = new ClassifiableCollection
        {
            data = new List<Classifiable>(),
        };

        private String ALPHABET = "-ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private String CLASSIFIABLES_EXIST = "Classifiables";
        private String CLASSIFIABLES_NONE = "No classifiables begin with that letter";
        private String ERROR_SERVER = "Sorry, there's an issue with the server!";
        private String REMOVE_SUCESS = "Sucessfully removed!";
        private String REMOVE_FAILED = "Could not remove.";
        private String FAILED_GENERAL = "There was some issue we did not catch";

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

        /// <summary>
        /// Fill in the ClassListBox with the classifiables that start with the provided
        /// letter
        /// </summary>
        /// <param name="letter">Starting letter of the classifiable name</param>
        protected void GenerateAlphaClassifiableList(char letter)
        {
            try
            {
                var dbConn = new Neo4jDB();
                alphaClassCollection = dbConn.getClassifiablesByAlphaGroup(letter);

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
                    // Hide the Classifiables listbox and show a message explaining
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

        protected void AlphabetDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(ALPHABET[AlphabetDDL.SelectedIndex]);

            int selectedLetterIndex = AlphabetDDL.SelectedIndex;

            // 0th index is the char '-', so don't do anything.
            if (selectedLetterIndex > 0)
            {
                GenerateAlphaClassifiableList(ALPHABET[AlphabetDDL.SelectedIndex]);
            }
        }

        protected void ClassListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedClassIndex = ClassListBox.SelectedIndex;

            if (selectedClassIndex >= 0 && selectedClassIndex < alphaClassCollection.data.Count)
            {
                TxtBxName.Text = alphaClassCollection.data[selectedClassIndex].name;
                TxtBxConStr.Text = alphaClassCollection.data[selectedClassIndex].conceptStr.ToString();
                TxtBxUrl.Text = alphaClassCollection.data[selectedClassIndex].url;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(String.Format(
                    "AdminRemoveClassOb_Index selected: {0:D}, Collection size: {1:D}",
                    selectedClassIndex, alphaClassCollection.data.Count));
            }
        }

        protected void ClearTextBoxFields()
        {
            TxtBxName.Text = "";
            TxtBxConStr.Text = "";
            TxtBxUrl.Text = "";
        }

        protected void BtnRemove_Click(object sender, EventArgs e)
        {
            int selectedIndex = ClassListBox.SelectedIndex;

            // Try to make sure a proper index is selected
            try
            {
                Classifiable toRemove = alphaClassCollection.data[selectedIndex];
                // Now try to remove it
                try
                {
                    var dbConn = new Neo4jDB();
                    dbConn.deleteClassifiable(toRemove);

                    Notification.Text = REMOVE_SUCESS;

                    ClearTextBoxFields();

                    // Re-Generate the list for that alphagroup
                    GenerateAlphaClassifiableList(ALPHABET[AlphabetDDL.SelectedIndex]);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    Notification.Text = REMOVE_FAILED;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Notification.Text = FAILED_GENERAL;
            }
            Notification.Visible = true;
        }
    }
}