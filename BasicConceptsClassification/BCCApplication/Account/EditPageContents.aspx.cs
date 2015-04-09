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
                ContactPreview.Text = LocalDataManager.Load(LocalDataManager.BCCContentFile.Contact);
            }
        }

        protected void AboutBoxSave_Click(object sender, EventArgs e)
        {
            LocalDataManager.Save(AboutBox.Text, LocalDataManager.BCCContentFile.About);
        }

        protected void ContactBoxSave_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            builder
                .Append(InformationBox.Text).Append("<hr />")
                .Append("Name: ").Append(NameBox.Text).Append("<br />")
                .Append("Phone: ").Append(PhoneBox.Text).Append("<br />")
                .Append("Email: <a href=\"mailto:").Append(EmailBox.Text.Trim()).Append("?subject=BCC:\">").Append(EmailBox.Text.Trim()).Append("</a><br />");

            LocalDataManager.Save(builder.ToString(), LocalDataManager.BCCContentFile.Contact);
        }
    }
}