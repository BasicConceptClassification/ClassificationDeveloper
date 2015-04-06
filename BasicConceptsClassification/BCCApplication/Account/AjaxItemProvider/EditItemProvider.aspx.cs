using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Goldtect;
using Neo4j;
using BCCLib;

namespace BCCApplication.Account.AjaxItemProvider
{
    public partial class EditItemProvider : System.Web.UI.Page
    {
        ASTreeViewAjaxReturnCode returnCode;
        string errorMessage = string.Empty;

        const string CUS_RETURN_CODE_OK = "0";
        const string CUS_RETURN_CODE_ERROR = "1";
        string cusReturnCode = "0";
        string cusMessage = "Edit Succeed!";

        //public string NodeText
        //{
        //    get
        //    {
        //        return HttpUtility.UrlDecode(Request.QueryString["nodeText"]);
        //    }
        //}

        public string NodeValue
        {
            get
            {
                return HttpUtility.UrlDecode(Request.QueryString["nodeValue"]);
            }
        }

        public string NewNodeText
        {
            get
            {
                return HttpUtility.UrlDecode(Request.QueryString["newNodeText"]);
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.NodeValue) || string.IsNullOrEmpty(this.NewNodeText))
            {
                returnCode = ASTreeViewAjaxReturnCode.ERROR;
                return;
            }

            try
            {
                if (this.NewNodeText.ToLower().IndexOf("a") >= 0)
                {
                    this.cusMessage = "[Server Said]:Node cannot contain the letter 'a'";
                    this.cusReturnCode = CUS_RETURN_CODE_ERROR;
                }
                else
                {
                    this.RenameNode();
                }
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
                writer.Write(((int)this.returnCode).ToString() + "|" + this.cusReturnCode + "|" + this.cusMessage);
            else
                writer.Write(this.errorMessage);
        }

        protected void RenameNode()
        {
            //get the string value from two text box
            string rename_from_string = NodeValue;
            string rename_to_string = NewNodeText;

            //open the Neo4j database
            var conn = new Neo4jDB();
            Term renameterm = conn.getTermByRaw(rename_from_string);
            string teststring1 = "";
            //won't let the page crush
            try
            {
                teststring1 = renameterm.ToString();
            }
            catch
            {

            }

            //set the input term to match with the term format in database
            renameterm.rawTerm = rename_to_string;
            renameterm.lower = rename_to_string.ToLower();

            //return the result to let user know.
            try
            {
                conn.renameTerm(renameterm, rename_to_string);
                // Notify classifiers that a Term was renamed.
                conn.createNotification(String.Format("Renamed Term: {0} to {1}.", rename_from_string, rename_to_string));
                // Regenerate the Controlled Vocabulary to see the changes
            }
            catch
            {

            }

        }
    }
}