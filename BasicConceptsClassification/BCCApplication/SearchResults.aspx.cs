using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SearchResults : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int x = 5;  //eventually tie to number of results in search thing

        for (int i = 0; i < x; i++)
        {
            Label ObName = new Label();
            ObName.ID = "ObName_" + i;
            ObName.Text = (i+1) + ". Object Name"; //Change to read name of object

            SearchReCon.Controls.Add(new LiteralControl("<strong>"));
            SearchReCon.Controls.Add(ObName);
            SearchReCon.Controls.Add(new LiteralControl("</strong><br/>"));

            int y = 3;
            for (int j = 0; j < y; j++)
            {
                Label NTag = new Label();
                NTag.ID = "Ob_" + i + "_Tag_" + j;
                NTag.Text = "Tag " + (j + 1) + ". ";  // Change to read tags of object

                SearchReCon.Controls.Add(NTag);
            }
            SearchReCon.Controls.Add(new LiteralControl("<br/> Source/Stored at: "));

            HyperLink ObLink = new HyperLink();
            ObLink.ID = "ObLink_" + i;
            ObLink.Target = "_blank";
            ObLink.Text = "http://www.something.com";  // Change to read object website
            ObLink.NavigateUrl = "Contact.aspx";  // Change to object website

            SearchReCon.Controls.Add(ObLink);
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));

        }
    }
}
