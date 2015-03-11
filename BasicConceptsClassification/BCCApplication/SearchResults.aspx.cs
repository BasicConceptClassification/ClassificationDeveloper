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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

    static List<objects> obj_results = new List<objects>();
    static List<objects> dis_results = new List<objects>();
    static Label ObName;
    static int counter_once = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        obj_results.Clear();
        // List<objects> dis_results = new List<objects>();

        // **** This section should not be done here! **** //
        // Using same example from the Neo4jTest.cs
        // CREATING TERMS FOR MAKING SAMPLE CONCEPT STRING 
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
        
        
        

    }

    protected void btnclick_Click(object sender, EventArgs e)
    {
        obj_results.Clear();
        dis_results.Clear();

        String user_searching = searching_textbox.Text;

        //string input_str = "[pig],[dog],[cat]";
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
            //ListBox1.Items.Add(things);
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
              

              //List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
              

              name = currentClassifiable.name;
              url = currentClassifiable.url;
              concept = currentClassifiable.conceptStr.ToString();
              terms_term = currentClassifiable.conceptStr.ToListstring();
              List<string> check_list = new List<string>();
              foreach (string things in terms_term)
              {
                  string newthings = things.Replace("(", "");
                  string new_t_things = newthings.Replace(")", "");
                  //ListBox1.Items.Add(new_t_things);
                  check_list.Add(new_t_things);
              }


              foreach (string items in new_str)
              {
                  //ListBox1.Items.Add(items);
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
                          //ListBox1.Items.Add(things);
                      }

                  }
              }

              //ListBox1.Items.Add("----------------------------");


              foreach (string items in new_str)
              {
                  set_s1++;
                  int set_s2 = 0;
                  //ListBox1.Items.Add("&&&&&&&&&" + items);

                  foreach (string things in counter_str)
                  {
                      set_s2++;
                      if (items == things)
                      {
                        //scores = scores + Math.Abs(set_s1 - set_s2);
                          if (set_s2 == set_s1)
                          {
                              scores = scores + 50;
                          }
                          else
                          {
                              scores = scores + 1;
                          }
                          //ListBox1.Items.Add(things);
                      }
                      
                  }
                  
              }
              


              //ListBox1.Items.Add(scores.ToString());
              int decreasecounter = counter;
              while (decreasecounter != 0)
              {
                  scores = scores + 100;
                  decreasecounter--;
              }
              //ListBox1.Items.Add(scores.ToString());
              //ListBox1.Items.Add("********************************");


              obj_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              dis_results.Add(new objects(name, terms_term, counter, concept, url, scores));
              
          }

          var sort_result = from element in obj_results orderby element.counter select element;
          int ind = 0;

          foreach (objects things in sort_result.Reverse())
          {

              //Classifiable currentClassifiable = searchResults.data[i];

              ObName = new Label();

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
              SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));//+things.counter.ToString()+

              ind++;

          }

    }

    protected void relev_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.counter select element;

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

    protected void name_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.name select element;

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
            SearchReCon.Controls.Add(new LiteralControl("<br/><br/>"));
            ind++;

        }
    }

    protected void order_sort_Click(object sender, EventArgs e)
    {
        int ind = 0;
        var sort_result = from element in dis_results orderby element.scor select element;

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

}
