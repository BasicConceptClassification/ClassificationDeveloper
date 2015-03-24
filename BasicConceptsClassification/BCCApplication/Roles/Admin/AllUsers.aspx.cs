using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BCCApplication.Roles.Admin
{
    public partial class AllUsers : System.Web.UI.Page
    {
        int pageSize = 5;
        int totalUsers;
        int totalPages;
        int currentPage = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            if( !IsPostBack )
            {
                GetUsers();
            }
        }

        private void GetUsers()
        {
            UsersOnlineLabel.Text = Membership.GetNumberOfUsersOnline().ToString();
            
            var allUsers = Membership.GetAllUsers(currentPage - 1, pageSize, out totalUsers);
            UserGrid.DataSource = allUsers;
            totalPages = ((totalUsers - 1) / pageSize) + 1;

            // Ensure that we do not navigate past the last page of users.

            if (currentPage > totalPages)
            {
                currentPage = totalPages;
                GetUsers();
                return;
            }

            UserGrid.DataBind();
            CurrentPageLabel.Text = currentPage.ToString();
            TotalPagesLabel.Text = totalPages.ToString();

            if (currentPage == totalPages)
                NextButton.Visible = false;
            else
                NextButton.Visible = true;

            if (currentPage == 1)
                PreviousButton.Visible = false;
            else
                PreviousButton.Visible = true;

            if (totalUsers <= 0)
                NavigationPanel.Visible = false;
            else
                NavigationPanel.Visible = true;
        }

        public void NextButton_OnClick(object sender, EventArgs args)
        {
            currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            currentPage++;
            GetUsers();
        }

        public void PreviousButton_OnClick(object sender, EventArgs args)
        {
            currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            currentPage--;
            GetUsers();
        }
    }
}