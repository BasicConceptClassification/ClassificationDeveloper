using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BCCLib;
using Neo4j;
using Goldtect;

namespace BCCApplication.Account.AjaxItemProvider
{
    public partial class AddItemProvider : System.Web.UI.Page
    {
        ASTreeViewAjaxReturnCode returnCode;
        string errorMessage = string.Empty;

        public string AddNodeText
        {
            get
            {
                return HttpUtility.UrlDecode(Request.QueryString["addNodeText"]);
            }
        }
        
        public string ParentNodeValue
        {
            get
            {
                return HttpUtility.UrlDecode(Request.QueryString["parentNodeValue"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.AddNodeText) || string.IsNullOrEmpty(this.ParentNodeValue))
            {
                returnCode = ASTreeViewAjaxReturnCode.ERROR;
                return;
            }

            try
            {
                this.AddNode();
            }
            catch (Exception ex)
            {
                this.returnCode = ASTreeViewAjaxReturnCode.ERROR;
                this.errorMessage = ex.Message;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.returnCode == ASTreeViewAjaxReturnCode.OK)
                writer.Write((int)this.returnCode);
            else
                writer.Write(this.errorMessage);
        }

        protected void AddNode()
        {
             //get the string value from two text box
            string new_term_string = AddNodeText;
            string parent_term = ParentNodeValue;

            //open the Neo4j database
            var conn = new Neo4jDB();

            //search the term by the rawterm
            Term searching_term = conn.getTermByRaw(parent_term);
            Term searching1_term = conn.getTermByRaw(new_term_string);

            if (searching1_term == null)
            {
                Term new_term = new Term
                {
                    id = new_term_string,
                    rawTerm = new_term_string,
                    lower = new_term_string.ToLower()
                };
                //conn.addTerm(new_term, searching_term);
                string teststring1 = "";

                //won't let the page crush

                try
                {
                    teststring1 = searching_term.ToString();
                }
                catch
                {

                }


                //return the result to let user know.

                if (teststring1 != "")
                {
                    conn.addTerm(new_term, searching_term);
                    // Notify classifiers that a Term was created.
                    conn.createNotification(String.Format("Added new Term: {0}", new_term.rawTerm));
                    // Regenerate the Controlled Vocabulary to see the changes
                }
            }
        }
    }
}