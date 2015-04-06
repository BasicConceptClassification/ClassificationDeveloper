using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using Neo4j;

namespace BCCApplication.Logic
{
    internal class RoleActions
    {
        public const string ROLE_ADMIN = "Administrator";
        public const string ROLE_CLASS = "Classifier";

        internal void AddUserAndRole()
        {
            // Access the application context and create result variables.
            Logic.ApplicationDbContext context = new ApplicationDbContext();
            IdentityResult IdRoleResult;
            IdentityResult IdUserResult;

            // Create a RoleStore object by using the ApplicationDbContext object. 
            // The RoleStore is only allowed to contain IdentityRole objects.
            var roleStore = new RoleStore<IdentityRole>(context);

            // Create a RoleManager object that is only allowed to contain IdentityRole objects.
            // When creating the RoleManager object, you pass in (as a parameter) a new RoleStore object.
            var roleMgr = new RoleManager<IdentityRole>(roleStore);

            // Then, you create the "canEdit" role if it doesn't already exist.
            if (!roleMgr.RoleExists(ROLE_ADMIN))
            {
                IdRoleResult = roleMgr.Create(new IdentityRole { Name = ROLE_ADMIN });
            }
            if (!roleMgr.RoleExists(ROLE_CLASS))
            {
                IdRoleResult = roleMgr.Create(new IdentityRole { Name = ROLE_CLASS });
            }

            // Create a UserManager object based on the UserStore object and the ApplicationDbContext  
            // object. Note that you can create new objects and use them as parameters in
            // a single line of code, rather than using multiple lines of code, as you did
            // for the RoleManager object.
            var userMgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var appUser = new ApplicationUser
            {
                UserName = "Admin",
                Email = "bcclassification@gmail.com",
            };
            IdUserResult = userMgr.Create(appUser, "password");

            // If the new "Admin" user was successfully created, 
            // add the "Admin" user to the "Admin" role. 
            if (!userMgr.IsInRole(userMgr.FindByName("Admin").Id, ROLE_ADMIN))
            {
                IdUserResult = userMgr.AddToRole(userMgr.FindByName("Admin").Id, ROLE_ADMIN);
            }

            // Not quite sure this should go here, but here is the Neo4j Sync
            try
            {
                var conn = new Neo4jDB();
                conn.updateAdmin(appUser.Email, appUser.UserName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}