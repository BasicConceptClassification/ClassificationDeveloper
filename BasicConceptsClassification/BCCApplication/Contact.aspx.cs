using Neo4j;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Contact : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Content.Text = LocalDataManager.Load(LocalDataManager.BCCContentFile.Contact);
        }
    }

    protected void CForm_Click(object sender, EventArgs e)
    {
        Response.Redirect("ContactForm.aspx", true);
    }
}