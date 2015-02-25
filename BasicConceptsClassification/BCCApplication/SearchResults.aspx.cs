using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BCCLib;
using Neo4j;

public partial class SearchResults : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
        Term termWood = new Term
        {
            rawTerm = "wood",
        };

        Term termTool = new Term
        {
            rawTerm = "Tool",
        };

        Term termFor = new Term
        {
            rawTerm = "for",
        };

        // CREATING SAMPLE CONCEPT STRING
        ConceptString searchByConStr = new ConceptString
        {
            terms = new List<Term> 
                {
                    termWood, termTool, termFor,
                },
        };
        // ********************************************* //

        // Searching for the concept string happens on this page?
        var dbConn = new Neo4jDB();
        ClassifiableCollection matchedClassifiables = dbConn.getClassifiablesByConStr(searchByConStr);

        // This part definately stays on this page. 
        // Assume a Classifiable collection gets passed to this page or it gets 
        // generated like above
        generateSearchResults(matchedClassifiables);
    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        printResults.Text = "You typed: " + testForYu.Text;
    }

    protected void generateSearchResults(ClassifiableCollection searchResults)
    {
        int resultsLength = searchResults.data.Count;  

        for (int i = 0; i < resultsLength; i++)
        {
            Classifiable currentClassifiable = searchResults.data[i];

            Label ObName = new Label();

            // Set this label to diaply the name of the Classifiable
            ObName.ID = "ObName_" + i;
            ObName.Text = String.Format("{0:D}. {1}", (i + 1), currentClassifiable.name);

            SearchReCon.Controls.Add(new LiteralControl("<strong>"));
            SearchReCon.Controls.Add(ObName);
            SearchReCon.Controls.Add(new LiteralControl("</strong><br/>"));

            // Create label for the concept string
            Label NTag = new Label();
            NTag.ID = "Ob_" + i + "_Tag_";
            NTag.Text = currentClassifiable.conceptStr.ToString();
            SearchReCon.Controls.Add(NTag);

            // Add hyperlink to the url of the Classifiable
            SearchReCon.Controls.Add(new LiteralControl("<br/> Source/Stored at: "));

            HyperLink ObLink = new HyperLink();
            ObLink.ID = "ObLink_" + i;
            ObLink.Target = "_blank";
            ObLink.Text = currentClassifiable.url;
            ObLink.NavigateUrl = currentClassifiable.url;

            SearchReCon.Controls.Add(ObLink);
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
        }
    }
}
