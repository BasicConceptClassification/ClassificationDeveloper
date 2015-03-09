using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BCCApplication.Account
{
    public partial class ClassOb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            string url = ObURL.Text;
            string name = ObName.Text;
            string concept = ObConcept.Text;
        }

        protected void SubmitObj_Click(object sender, EventArgs e)
        {

        }
    }
}