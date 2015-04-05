using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Neo4j;
using System.Text;

namespace BCCApplication.Account
{
    public partial class EditPageContents : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AboutBox.Text = LocalDataManager.Load(LocalDataManager.BCCContentFile.About);
            }
        }

        protected void AboutBoxSave_Click(object sender, EventArgs e)
        {
            LocalDataManager.Save(AboutBox.Text, LocalDataManager.BCCContentFile.About);
        }
    }
}