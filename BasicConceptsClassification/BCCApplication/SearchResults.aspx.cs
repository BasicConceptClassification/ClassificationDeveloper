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
    public int scor;
    public string concept;
    public string url;
    public objects(string s, List<string> l, int c, string se, string u, int sc)
    {
        this.name = s;
        this.terms_term = l;
        this.counter = c;
        this.concept = se;
        this.url = u;
        this.scor = sc;
    }
}

public partial class SearchResults : System.Web.UI.Page
{
    private string DESCRIPTION = @"<p>To view the search results, please click the Search button below.
                                    You can choose order you want the results in by selecting any one of
                                    the <em>Sort By...</em> butttons below.</p>";

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    
    /// <summary>
    /// load the page of it is get accessed by the search page will display the searching detial on text box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <return> won't return any thing </return>

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();

        if (counter_once == 1)
        {
            string getinput = "";
            try
            {
                getinput = Application["textpass"].ToString();
            }
            catch
            {

            }
            searching_textbox.Text = getinput;
            counter_once--;

        }
        
        if(!Page.IsPostBack)
        {
            LabelDescription.Text = DESCRIPTION;
        }
        
        

    }

    /// <summary>
    /// when we click the search button and will display the results
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        string Triminput_str = user_searching.Trim();
        string sstring = Triminput_str.Replace(")(", ",");
        sstring = sstring.Replace(")", "");
        sstring = sstring.Replace("(", "");
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
          int resultsLength = matchedClassifiables.data.Count;

          for (int i = 0; i < resultsLength; i++)
          {
              Classifiable currentClassifiable = matchedClassifiables.data[i];

              string name;
              List<string> terms_term;
              int counter =0;
              string concept;
              string url;
              int scores = 0;

              int set_s1 = 0;

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();

              //remove the ( ) things
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  foreach (string thing in check_list)
                  {
                      if (items == thing)
                      {
                          counter = counter + 1;
                      }
                  }
              }


              List<string> counter_str = new List<string>();

              foreach(string items in new_str)
              {
                  foreach (string things in check_list)
                  {
                      if (items == things)
                      {
                          counter_str.Add(things);
                      }

                  }
              }


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          
                      }
                      
                  }
                  
              }
              
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }

              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
          }

          string cases = "nothing";
          display(cases);

    }

    /// <summary>
    /// when click the sort by relev button it will sort the searching result as the relevent terms.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void relev_sort_Click(object sender, EventArgs e)
    {
        string cases = "relev";
        display(cases);
    }

    /// <summary>
    /// when click the sort by name button it will sort the searching result as the name.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void name_sort_Click(object sender, EventArgs e)
    {
        string cases = "name";
        display(cases);
    }

    /// <summary>
    /// when click the sort by order of term button it will sort the searching result as the order of relevent terms.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void order_sort_Click(object sender, EventArgs e)
    {
        string cases = "order";
        display(cases);

    }

    /// <summary>
    /// for different sort button display in different way
    /// </summary>
    /// <param name="cases"></param>
    protected void display(string cases)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;
        if(cases == "order")
        {
             sort_result = from element in dis_results orderby element.scor select element;
        }

        if (cases == "name")
        {
            sort_result = from element in dis_results orderby element.name select element;
            foreach (objects things in sort_result.ToList())
            {

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
        else
        {
            foreach (objects things in sort_result.Reverse().ToList())
            {

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

        if (cases == "relev")
        {
            sort_result = from element in dis_results orderby element.counter select element;
        }

        
    }
    
}
