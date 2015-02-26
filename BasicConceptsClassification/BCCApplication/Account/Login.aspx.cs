using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;

namespace BCCApplication.Account
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register";
            OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];

            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            }
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Validate the user password
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

                string un = LoginForm.UserName;
                string pw = LoginForm.Password;
                bool remember = LoginForm.RememberMeSet;

                var result = signinManager.PasswordSignIn(un, pw, remember, shouldLockout: true);

                switch (result)
                {
                    case SignInStatus.Success:
                        Response.Redirect("/");
                        break;
                    case SignInStatus.LockedOut:
                        //Response.Redirect("/Account/Lockout");
                        break;
                    case SignInStatus.RequiresVerification:
                        break;
                    case SignInStatus.Failure:
                    default:
                        break;
                }
            }
        }
    }
}