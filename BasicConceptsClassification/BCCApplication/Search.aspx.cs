using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using BCCLib;
using Neo4j;

using System.Diagnostics;

public partial class Search : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        // Testing purposes, only loading from BccRoot with a small depth
        int expandDepth = 2;

        // Fetch BCC from the DB
        var dbConn = new Neo4jDB();
        Term bccRootTerm = dbConn.getBccFromRootWithDepth(expandDepth);

        // Create a starting TreeNode as the root to generate the BCC
        TreeNode currentNode = new TreeNode();
        DataSet.Nodes.Add(generateBccTree(bccRootTerm, currentNode));

        // By default, leave collapsed
        DataSet.CollapseAll();
    }


    protected void DataSet_SelectedNodeChanged(object sender, EventArgs e)
    {

    }

    protected TreeNode generateBccTree(Term t, TreeNode currentNode)
    {
        String nodeTerm = t.rawTerm;

        // Create/set the current node we're on: the text displayed with the
        // node and the action when selected.
        currentNode = new TreeNode(nodeTerm);
        currentNode.SelectAction = TreeNodeSelectAction.None;

        // Foreach child, recursively build this up
        foreach (var childTerm in t.subTerms)
        {    
            currentNode.ChildNodes.Add(generateBccTree(childTerm, currentNode));
        }

        return currentNode;
    }
}