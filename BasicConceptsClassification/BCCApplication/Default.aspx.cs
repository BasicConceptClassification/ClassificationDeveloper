﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using Neo4j;
using BCCLib;

namespace BCCApplication
{
    public partial class _Default : Page
    {
        // How much the tree should be expanded by when it needs to expand.
        static int EXPAND_DEPTH = 2;

        private string DESCRIPTION = @"<p>Welcome to the Basic Concepts Classification. Here you can search for items found in the 
                                        galleries, archieves, and museums (GLAM). Each Item can be searched for by terms in the classification.</p>
                                        <p>Use the menu at the top of the to learn more about the Classification, start searching,
                                        or contact the admin to help classify GLAM objects today!</p>";

        private static string ERROR_SERVER = "Sorry, there was an error with the server.";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LabelDescription.Text = DESCRIPTION;
                GenerateInitialBCCTree();
            }
        }

        protected void GenerateInitialBCCTree()
        {
            // If server is down, display an error message
            try
            {
                // Fetch BCC from the DB
                var dbConn = new Neo4jDB();
                Term bccRootTerm = dbConn.getBccFromRootWithDepth(EXPAND_DEPTH);

                DataSet.Nodes.Clear();
                // Create a starting TreeNode as the root to generate the BCC
                TreeNode currentNode = new TreeNode();
                DataSet.Nodes.Add(generateBccTree(bccRootTerm, currentNode));

                // By default, leave collapsed
                DataSet.CollapseAll();
                DataSet.ShowCheckBoxes = TreeNodeTypes.Leaf;
                LabelNoticationDataSet.Text = "";
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine(Ex.Message);
                LabelNoticationDataSet.Text = ERROR_SERVER;
            }
        }

        /// <summary>
        /// Raised when a node is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DataSet_SelectedNodeChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DataSet.SelectedNode.Text);
        }
        
        /// <summary>
        /// Converts a Term and its subTerms/children Terms to a TreeNode. 
        /// </summary>
        /// <param name="currentTerm">Current Term in the recursion.</param>
        /// <param name="currentNode">Current TreeNode in the recursion.</param>
        /// <returns>TreeNode reprentation of a Term.</returns>
        protected TreeNode generateBccTree(Term currentTerm, TreeNode currentNode)
        {
            String nodeTerm = currentTerm.rawTerm;

            // Create/set the current node we're on: the text displayed with the
            // node and the action when selected.
            currentNode = new TreeNode(nodeTerm);
            currentNode.SelectAction = TreeNodeSelectAction.None;

            // Sort subTerms by alphabetical order
            currentTerm.sortSubTerms();

            if (currentTerm.subTerms.Count > 0)
            {
                // Foreach child, recursively build this up
                foreach (var childTerm in currentTerm.subTerms)
                {
                    currentNode.ChildNodes.Add(generateBccTree(childTerm, currentNode));
                }
            }
            else
            {
                // If there are no children, then there's a chance to populate!
                currentNode.PopulateOnDemand = true;
            }
            return currentNode;
        }

        protected void SearchBtn_Click(object sender, EventArgs e)
        {
            string str = TextBox2.Text;
            Application["textpass"] = str;
            Response.Redirect("~/SearchResults.aspx", true);
        }

        /// <summary>
        /// Populates the DataSet BCC Tree OnDemand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PopulateNode(Object sender, TreeNodeEventArgs e)
        {
            TreeNode currentNode = e.Node;

            Term reference = new Term
            {
                rawTerm = currentNode.Text,
            };

            var dbConn = new Neo4jDB();
            try
            {
                Term bccRootTerm = dbConn.getBccFromTermWithDepth(reference, EXPAND_DEPTH);
                foreach (var term in bccRootTerm.subTerms)
                {
                    // Create a starting TreeNode as the root to generate the BCC
                    currentNode.ChildNodes.Add(generateBccTree(term, new TreeNode()));
                }
                LabelNoticationDataSet.Text = "";
            }
            catch
            {
                // This doesn't show up... but it's here in case it might?
                LabelNoticationDataSet.Text = ERROR_SERVER;
            }
        }
    }
}