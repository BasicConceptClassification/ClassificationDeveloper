using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;

using BCCApplication;
using BCCApplication.Logic;

namespace BCCApplication
{
    public class Global : HttpApplication
    {
        public const string ROLE_DEFAULT = "DefaultRole";
        public const string ROLE_CLASSIFY = "ClassifyRole";
        public const string ROLE_ADMIN = "AdminRole";

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AuthConfig.RegisterOpenAuth();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            RoleActions roles = new RoleActions();
            //roles.AddUserAndRole();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        
    }
}
