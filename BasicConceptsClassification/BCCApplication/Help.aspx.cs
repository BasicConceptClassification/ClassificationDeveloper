using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BCCApplication.Logic;

public partial class Help : System.Web.UI.Page
{
    private string DESCRIPTION_REG = @"<h3>What can you do?</h3>
                                        <p>Anyone can use this site to search for any GLAM objects that have
                                        been registered by GLAMs all over the world. Just click on Search from the
                                        top menu to get started.</p>
                                        <p>You can also register an account with us if you're interested in leanring
                                        more about us. If you wish to be apart of classifying GLAM objects, you
                                        can visit the <em>Contact Us</em> page to contact the Admin to get an
                                        account created.</p>";

    // Vocabulary should go on this page? Here is a simple example.
    private string VOCABULARY = @"<h3>Glossary</h3>
                                    <ul>
                                        <li>GLAM: Stands for Galleries, Libraries, Archives and Museums.</li>
                                        <li>Term: Are used to classify GLAM objects.</li>
                                    </ul>";

    private string DESCRIPTION_CLASS = @"<h3>What can a Classifier do?</h3>
                                        <p>A Classifier can add GLAM objects from their GLAM that have not been
                                        added to this site so they can be found by other users of this site.
                                        You can do this by clicking <em>Classification Tools</em> at the top of
                                        the page, then clicking on the <em>Add</em> option.
                                        You can choose to classify them as soon as you add them or you can leave
                                        the Concept String empty and then you can classify it later.
                                        You can also choose to only let yourself classify or edit each GLAM object
                                        or allow other Classifiers in your GLAM to help you classify your
                                        GLAM objects.</p>
                                        <p>If you need to remove any GLAM object you have added, you can also do that
                                        by going to <em>Classification Tools</em> and then select the <em>Remove</em>
                                        option. This will take you to a separate page where you can choose which
                                        GLAM object you have added to be removed. Once you remove it, you cannot undo
                                        this action! You will have to add the GLAM object again.</p>
                                        <p>For each of these actions, please visit the respective page to get more
                                        information about it.</p>";

    private string DESCRIPTION_ADMIN = @"<h3>What can an Admin do?</h3>
                                        <p>The Admin can register new users as Classifiers or as regular users by clicking 
                                        <em>Add a new user</em> at the top of the page.</p>
                                        <p>The Admin can modify the Terms in the Controlled Vocabulary by clicking
                                        <em>Modify Terms</em> at the top of the page. On that page, you can <em>Add</em> new Terms,
                                        <em>Move</em> where Terms are located, <em>Rename</em> Terms and you can also <em>Delete</em>
                                        Terms. Notifications will be sent to all Classifiers whenever you Add, Rename or Delete a Term.</p>
                                        <p>The Admin can remove <em>any</em> GLAM object that has been added by any Classifier with
                                        any GLAM. A notification will be sent to the Classifier if you remove one of their GLAM objects.</p>
                                        <p>For each of these actions, please visit the respective page to get more
                                        information about it.</p>";

    // TODO: Question: Do we have detailed help information here and have simplified help on the respective page
    // OR
    // Have the same information on both pages?
    // Right now there is some help on the search page, and I don't feel like copying that over now.
    private string HELP_SEARCH = @"";

    protected void Page_Load(object sender, EventArgs e)
    {
        // Load content for the page based on what type of user is currently logged in.
        LabelDescription.Text = DESCRIPTION_REG;

        if(User.IsInRole(RoleActions.ROLE_ADMIN))
        {
            LabelDescription.Text += DESCRIPTION_ADMIN;
        }
        else if (User.IsInRole(RoleActions.ROLE_CLASS))
        {
            LabelDescription.Text += DESCRIPTION_CLASS;
        }

        LabelDescription.Text += VOCABULARY;
    }
}