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
            if (User.Identity.IsAuthenticated)
            {
                // if they came to the page directly, ReturnUrl will be null.
                if (String.IsNullOrEmpty(Request["ReturnUrl"]))
                {
                    /* in that case, instead of redirecting, I hide the login 
                       controls and instead display a message saying that are 
                       already logged in. */
                }
                else
                {
                    Response.Redirect("~/Account/AccessDenied.aspx");
                }
            }

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
                        IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                        break;
                    case SignInStatus.LockedOut:
                    case SignInStatus.RequiresVerification:
                    case SignInStatus.Failure:
                    default:
                        break;
                }
            }
        }
    }
}