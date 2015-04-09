using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Neo4j;
using Goldtect;
using BCCLib;

namespace BCCApplication.Account.AjaxItemProvider
{
    public partial class DeleteItemProvider : System.Web.UI.Page
    {
        ASTreeViewAjaxReturnCode returnCode;
        string errorMessage = string.Empty;

        public string DeleteNodeValues
        {
            get
            {
                return HttpUtility.UrlDecode(Request.QueryString["deleteNodeValues"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.DeleteNodeValues))
            {
                returnCode = ASTreeViewAjaxReturnCode.ERROR;
                return;
            }

            try
            {
                this.DeleteNodes();
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

        protected void DeleteNodes()
        {
             //get the string value from text box
            string delete_term = DeleteNodeValues;

            //open the Neo4j database
            var conn = new Neo4jDB();
            Term delete_search_term = conn.getTermByRaw(delete_term);
            string teststring1 = "";
            //won't let the page crush\
            
            try
            {
                teststring1 = delete_search_term.ToString();
            }
            catch
            {

            }

            //return the result to let user know.
            if (teststring1 != "")
            {
                Neo4jDB.AffectedNodes affectedClassifiables = new Neo4jDB.AffectedNodes();
                try
                {
                    // Commented out for testing this part!
                    affectedClassifiables = conn.delTerm(delete_search_term);
                    
                    // Notify classifiers that a Term was deleted.
                    // TODO: create additional notification to the classifiers whose classifiable's ConStr
                    // were affect by the Term deletion.
                    conn.createNotification(String.Format("Removed Term: {0}", delete_search_term));
                    // Regenerate the Controlled Vocabulary to see the changes
                }
                catch
                {

                }

                
                try 
                {
                    if (affectedClassifiables.classifiablesAffected.Count > 0)
                    {
                        foreach (var classObj in affectedClassifiables.classifiablesAffected)
                        {
                            // get each by id and update
                            Classifiable toUpdate = conn.getClassifiableById(classObj.id);
                            Classifiable old = toUpdate;
                            if (toUpdate != null)
                            { 
                                // what if a term appears multiple times?
                                toUpdate.conceptStr.terms.Remove(delete_search_term);
                                toUpdate.status = Classifiable.Status.AdminModified.ToString();
                            }
                            // go through the string of terms and when equals the one removed, don't add
                            // then call update
                            conn.updateClassifiable(old, toUpdate, old.owner);
                            conn.createNotification(
                                String.Format("Admin has modified the concept string of {0}",toUpdate.name), 
                                old.owner.email);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}