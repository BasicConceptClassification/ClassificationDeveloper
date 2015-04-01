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

        private string DESCRIPTION = @"<p>To remove a GLAM object, first select the starting letter of the GLAM object 
                                        to get a list of all GLAM objects that start with that letter. If you want to get all
                                        the GLAM Objects that don't start a letter of the alphabet, select the '#' at the
                                        bottom of the list.</p>
                                        <p>Then click on the GLAM object from the list to view the information about the GLAM object. 
                                        Finally, click the the remove button to remove the GLAM object. </p>
                                        <p>A notification will be sent to the owner of that GLAM object to inform them about the removal.</p>";

        private String ALPHABET = "-ABCDEFGHIJKLMNOPQRSTUVWXYZ#";

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
                LabelDescription.Text = DESCRIPTION;

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

        /// <summary>
        /// When the user selects a letter from the dropdown list, goes to fetch the classifiables that start with that letter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// When the user selects a classifiable from the list of them, fill in that classifiable's data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ClassListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedClassIndex = ClassListBox.SelectedIndex;

            // Check for out of range, just in case.
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

        /// <summary>
        /// On RemoveButton click, attempts to remove that classifiable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                    // TODO: FIX. Don't query the DB again for owner's email?
                    Classifier owner = dbConn.getClassifiableById(toRemove.id).owner;

                    System.Diagnostics.Debug.WriteLine("AdminRemoveClassOb_Removing: {0}; Owner: {1}", toRemove.id, owner.email);

                    dbConn.deleteClassifiable(toRemove);
                    dbConn.createNotification(
                        String.Format("Admin has removed your GLAM Object called {0}.", toRemove.name), 
                        owner.email);

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