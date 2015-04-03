using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using Microsoft.AspNet.Membership.OpenAuth;
using BCCApplication.Logic;
using Microsoft.AspNet.Identity.EntityFramework;

using Neo4j;
using BCCLib;
using System.Diagnostics;

namespace BCCApplication.Account
{
    public partial class Register : Page
    {
        static List<GLAM> existingGLAMS = new List<GLAM>();

        // Brandon: here's an idea for having different written content on the site.
        // Most pages won't have two 'versions' to load though. And I'm not sure how you'd
        // want to handle the formating.
        static string REG_USR_DESC = "Please enter in your information into the fields below. Passwords must be at least 6 characters in length.";
        static string ADMIN_USR_DESC  = @"When creating a new Classifier, please click the checkbox called Make Classifier.
                                                If you wish to associate them with a previously added GLAM, please select one from the list.
                                                If you wish to associate them with a new GLAM, please click the checkbox Create New GLAM and 
                                                then type in the name of the new GLAM.";

        static string NEWGLAM_BLANK = "Please enter in the name of the new GLAM or uncheck the Create New GLAM checkbox to use one from the list.";

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                if (User.IsInRole(RoleActions.ROLE_ADMIN))
                {
                    AdminVisibility(true);
                    LabelDescription.Text = REG_USR_DESC + ADMIN_USR_DESC;

                    // Generate GLAMs to pick from
                    try
                    {
                        var dbConn = new Neo4jDB();
                        existingGLAMS = dbConn.getAllGlams();
                        if (existingGLAMS.Count != 0)
                        {
                            foreach (GLAM g in existingGLAMS)
                            {
                                GLAMListBox.Items.Add(g.name);
                            }
                            GLAMListBox.SelectedIndex = 0;
                            GLAMListBox.Visible = true;
                            LabelGLAMListBox.Visible = true;
                        }
                    }
                    catch (Exception) { }
                }
                else
                {
                    AdminVisibility(false);
                    LabelDescription.Text = REG_USR_DESC;
                }
            }
        }

        /// <summary>
        /// Change the visibility of some controls based on Role.
        /// </summary>
        /// <param name="isAdmin">If user is admin, set to true.</param>
        protected void AdminVisibility(bool isAdmin)
        {
            // Ability to make the new user a classifier
            ClassifierCheckBox.Visible = isAdmin;
            LabelClassifierCheckBox.Visible = isAdmin;

            // Ability to select from an existing GLAM
            LabelGLAMListBox.Visible = isAdmin;
            GLAMListBox.Visible = isAdmin;

            // Ability to create a new GLAM
            CheckBxCreateNewGLAM.Visible = isAdmin;
            LabelCheckBxCreateNewGLAM.Visible = isAdmin;
            TxtBxNewGLAM.Visible = isAdmin;
        }

        protected void CreateUser_Click(object sender, EventArgs e)
        {

            // There are ceratin checks that need to happen before any user is added
            // and it's harder with there being 2 DBs to keep in sync...
            if (CheckBxCreateNewGLAM.Checked && TxtBxNewGLAM.Text == "")
            {
                LabelNotification.Text = NEWGLAM_BLANK;
                LabelNotification.Visible = true;
            }
            else
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
                var user = new ApplicationUser() { UserName = Username.Text, Email = Email.Text };
                IdentityResult IdUserResult = manager.Create(user, Password.Text);

                if (IdUserResult.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    //string code = manager.GenerateEmailConfirmationToken(user.Id);
                    //string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);
                    //manager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.");

                    if (ClassifierCheckBox.Checked && User.IsInRole(RoleActions.ROLE_ADMIN))
                    {
                        // Load the role manager and add the user to the classifiers.
                        Logic.ApplicationDbContext context = new ApplicationDbContext();
                        IdentityResult IdRoleResult;
                        var roleStore = new RoleStore<IdentityRole>(context);
                        var roleMgr = new RoleManager<IdentityRole>(roleStore);
                        if (!roleMgr.RoleExists(RoleActions.ROLE_CLASS))
                        {
                            IdRoleResult = roleMgr.Create(new IdentityRole { Name = RoleActions.ROLE_CLASS });
                        }
                        var userMgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                        if (!userMgr.IsInRole(userMgr.FindByName(Username.Text).Id, RoleActions.ROLE_CLASS))
                        {
                            IdUserResult = userMgr.AddToRole(userMgr.FindByName(Username.Text).Id, RoleActions.ROLE_CLASS);
                        }

                        AddNeo4jClassifierData();

                    }
                    signInManager.PasswordSignIn(Username.Text, Password.Text, true, false);

                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);

                }
                else
                {
                    ErrorMessage.Text = IdUserResult.Errors.FirstOrDefault();
                }
            }
        }

        protected int AddNeo4jClassifierData()
        {
            string glamName = "";
            if (CheckBxCreateNewGLAM.Checked)
            {
                glamName = TxtBxNewGLAM.Text;

                // "" should already by checked by a check outside, but...
                if (glamName == "") return 1;
            } 
            else
            {
                glamName = GLAMListBox.SelectedValue;
            }

            GLAM classifierGlam = new GLAM(glamName);
            Classifier newClassifier = new Classifier(classifierGlam, Email.Text, Username.Text);

            // Need to something more than this?
            try
            {
                var dbConn = new Neo4jDB();
                dbConn.addClassifier(newClassifier);
                return 0;
            }
            catch (ArgumentNullException ex) 
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}