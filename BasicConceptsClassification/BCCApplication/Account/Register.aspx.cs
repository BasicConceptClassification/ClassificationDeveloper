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
        protected void Page_Load(object sender, EventArgs e)
        {


            if (User.IsInRole(RoleActions.ROLE_ADMIN))
            {
                ClassifierCheckBox.Visible = true;
                LabelClassifierCheckBox.Visible = true;

                // Generate GLAMs to pick from
                try
                {
                    var dbConn = new Neo4jDB();
                    List<GLAM> GLAMS = dbConn.getAllGlams();
                    if (GLAMS.Count != 0)
                    {
                        foreach (GLAM g in GLAMS)
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
                ClassifierCheckBox.Visible = false;
                LabelClassifierCheckBox.Visible = false;
                LabelGLAMListBox.Visible = false;
                GLAMListBox.Visible = false;
            }
        }

        protected void CreateUser_Click(object sender, EventArgs e)
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

        protected int AddNeo4jClassifierData()
        {
            GLAM classifierGlam = new GLAM(GLAMListBox.SelectedValue);
            Classifier newClassifier = new Classifier(classifierGlam);
            newClassifier.email = Email.Text;

            // Need to something more than this?
            try
            {
                var dbConn = new Neo4jDB();
                dbConn.addClassifier(newClassifier);
                return 0;
            }
            catch (Exception) 
            {
                return 1;
            }
        }
    }
}