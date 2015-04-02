using System;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Web.Security;

namespace BCCApplication.Account
{
    public partial class AdminViewAllUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int numRecords;
            MembershipUserCollection users = Membership.GetAllUsers(0, 100, out numRecords);

            GridView.DataSource = users;
            GridView.DataBind();
        }
    }
}