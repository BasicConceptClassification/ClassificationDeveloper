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

    static List<objects> obj_results = new List<objects>();
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
        List<string> new_str = Triminput_str.Split(',').ToList();

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
          int resultsLength = matchedClassifiables.data.Count;

          for (int i = 0; i < resultsLength; i++)
          {
              Classifiable currentClassifiable = matchedClassifiables.data[i];

              string name;
              List<string> terms_term;
              int counter =0;
              string concept;
              string url;

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();

              foreach (string items in new_str)
              {
                  foreach (string thing in terms_term)
                  {
                      if (items == thing)
                      {
                          counter = counter + 1;
                      }
                  }
              }

              obj_results.Add(new objects(name, terms_term, counter, concept, url));
              
          }

          var sort_result = from element in obj_results orderby element.name select element;
          int ind = 0;

          foreach (objects things in sort_result)
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              Label ObName = new Label();

              // Set this label to diaply the name of the Classifiable
              ObName.ID = "ObName_" + ind;
              ObName.Text = String.Format("{0:D}. {1}", (ind + 1), things.name);

              SearchReCon.Controls.Add(new LiteralControl("<strong>"));
              SearchReCon.Controls.Add(ObName);
              SearchReCon.Controls.Add(new LiteralControl("</strong><br/>"));

              // Create label for the concept string
              Label NTag = new Label();
              NTag.ID = "Ob_" + ind + "_Tag_";
              NTag.Text = things.concept;
              SearchReCon.Controls.Add(NTag);

              // Add hyperlink to the url of the Classifiable
              SearchReCon.Controls.Add(new LiteralControl("<br/> Source/Stored at: "));

              HyperLink ObLink = new HyperLink();
              ObLink.ID = "ObLink_" + ind;
              ObLink.Target = "_blank";
              ObLink.Text = things.url;
              ObLink.NavigateUrl = things.url;

              SearchReCon.Controls.Add(ObLink);
              SearchReCon.Controls.Add(new LiteralControl("<br/>"+things.counter.ToString()+"<br/>"));

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in obj_results orderby element.counter select element;

        foreach (objects things in sort_result.Reverse().ToList())
        {

            //Classifiable currentClassifiable = searchResults.data[i];

            Label ObName = new Label();

            // Set this label to diaply the name of the Classifiable
            ObName.ID = "ObName_" + ind;
            ObName.Text = String.Format("{0:D}. {1}", (ind + 1), things.name);

            SearchReCon.Controls.Add(new LiteralControl("<strong>"));
            SearchReCon.Controls.Add(ObName);
            SearchReCon.Controls.Add(new LiteralControl("</strong><br/>"));

            // Create label for the concept string
            Label NTag = new Label();
            NTag.ID = "Ob_" + ind + "_Tag_";
            NTag.Text = things.concept;
            SearchReCon.Controls.Add(NTag);

            // Add hyperlink to the url of the Classifiable
            SearchReCon.Controls.Add(new LiteralControl("<br/> Source/Stored at: "));

            HyperLink ObLink = new HyperLink();
            ObLink.ID = "ObLink_" + ind;
            ObLink.Target = "_blank";
            ObLink.Text = things.url;
            ObLink.NavigateUrl = things.url;

            SearchReCon.Controls.Add(ObLink);
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

/*
    protected void generateSearchResults(List<objects> searchResults)
    {
        //int resultsLength = searchResults.data.Count;  
        
    }*/


}
