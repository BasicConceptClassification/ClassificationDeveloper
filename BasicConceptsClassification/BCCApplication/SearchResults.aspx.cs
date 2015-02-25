using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BCCLib;
using Neo4j;




class objects
{
    public string name;
    public List<string> terms_term;
    public int counter;
    public string concept;
    public string url;
    public objects(string s, List<string> l, int c, string se, string u)
    {
        this.name = s;
        this.terms_term = l;
        this.counter = c;
        this.concept = se;
        this.url = u;
    }
}

public partial class SearchResults : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
        
    }

    protected void btnclick_Click(object sender, EventArgs e)
    {

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
        string Triminput_str = user_searching.Trim();
        string sstring = Triminput_str.Replace("(", "");
        sstring = sstring.Replace(")", "");
        List<string> new_str = sstring.Split(',').ToList();

        List<Term> new_terms = new List<Term>();

        foreach (String things in new_str)
        {
            //change to terms
            Term terterma = new Term{rawTerm = things,};
            new_terms.Add(terterma);
        }

        ConceptString searchByConStr = new ConceptString
        {
            terms = new_terms,

        };
        // ********************************************* //

        // Searching for the concept string happens on this page?
        var dbConn = new Neo4jDB();
        ClassifiableCollection matchedClassifiables = dbConn.getClassifiablesByConStr(searchByConStr);

        // This part definately stays on this page. 
        // Assume a Classifiable collection gets passed to this page or it gets 
        // generated like above
        generateSearchResults(matchedClassifiables);




        //----------------------------------------------------------------------
        //-----------------searching part --------------------------
        /*
        int id = 2;

        Classifiable classy = dbConn.getClassifiableById(id);
        


        string name;
        List<string> terms_term;
        int counter = 0;
        string concept;
        string url;

        name = classy.name;
        url = classy.url;
        concept = classy.conceptStr.ToString();
        terms_term = classy.conceptStr.ToListstring();
        results_box.Items.Add(name);
        results_box.Items.Add(url);
        results_box.Items.Add(concept);
        foreach (String things in terms_term)
        {
            results_box.Items.Add(things);
        }

        foreach (String things in new_str)
        {
            results_box.Items.Add(things);
        }*/
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
