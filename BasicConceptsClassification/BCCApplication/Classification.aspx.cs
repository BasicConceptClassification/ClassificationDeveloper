using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BCCApplication
{
    public partial class Classification : System.Web.UI.Page
    {
        private string DESCRIPTION = @"<p>Learn all about the Classification on this page!</p>
                                        <p>Galleries, Libraries, Archives and Museums (GLAMs) around the world have databases that are ineffective to search
                                        through due to isolated systems and a non-uniform vocabulary. <b><i>Basic Concepts Classification</i></b> hopes
                                        to address both these problems with an easy to learn platform for classification and searching of the collections
                                        of GLAMs around the world.</p>
                                        <p>With this system, curators from different institutions will be able to add items to a centralized database, and all
                                        users will be able to search through this database.</p>";
        protected void Page_Load(object sender, EventArgs e)
        {
            LabelDescription.Text = DESCRIPTION;
        }
    }
}