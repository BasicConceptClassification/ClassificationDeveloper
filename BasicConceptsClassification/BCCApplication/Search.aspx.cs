using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Search : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int dataObjects = 2; // Have it as number of objects in database
        int i = 0; //used to figure out if it has found all objects


        TreeNode currentNode = new TreeNode();
        TreeNode tempNode;
        while (i < dataObjects)
        {
            //if(has child) {
            String currentTag = "Tag";

            if (currentNode.Parent == null)
            {
                currentNode = new TreeNode(currentTag);

                currentNode.SelectAction = TreeNodeSelectAction.None;
                DataSet.Nodes.Add(currentNode);
            }
            else
            {
                tempNode = new TreeNode(currentTag);

                tempNode.SelectAction = TreeNodeSelectAction.None;
                currentNode.ChildNodes.Add(tempNode);

                currentNode = tempNode;
            }
            // Will need to figure out how to implement selectAction to add tag to search string

            TreeNode tempCont = new TreeNode(currentTag);
            //}
            //else {
            // basically this is if it finds a object instead of a tag (thus a non-parent object or whatever)
            String currentObName = "I need a Name from Database"; // have it as object name
            String currentObHTML = "http://www.something.com"; // have it as object html
            String childLabel = "<b>" + currentObName + "</b><br/>Source/Stored at: ";
            childLabel = childLabel + "<a href='" + "'>" + currentObHTML + "</a>"; // Going to have to add currentObHTML between "<a... and "'/>"

            TreeNode childNodeThing = new TreeNode(childLabel);
            childNodeThing.SelectAction = TreeNodeSelectAction.None;

            currentNode.ChildNodes.Add(childNodeThing);

            i = i + 1;
            //}

            // need to return to parent of next tag
            //while (nextTag.parent.Text != currentNode.Text) {
            //  currentNode = currentNode.parent;
            //}
        }
        DataSet.CollapseAll();
    }


    protected void DataSet_SelectedNodeChanged(object sender, EventArgs e)
    {

    }
}