using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Neo4j;
using BCCLib;
using System.Linq;



namespace BCCApplication.Neo4j_text
{
    class objects_example
    {
        public string name;
        public List<string> terms_term;
        public int counter;
        public string concept;
        public string url;
        public objects_example(string s, List<string> l, int c,string se, string u)
        {
            this.name = s;
            this.terms_term = l;
            this.counter = c;
            this.concept = se;
            this.url = u;
        }
    }




    public partial class WebForm1 : System.Web.UI.Page
    {
        public static string inputstr_add = "";
        

        protected void Page_Load(object sender, EventArgs e)
        {
            

        }

        protected void Cleck_Button_Click(object sender, EventArgs e)
        {
            String show = Input_text.Text;
            Response.Write(show);
        }

        protected void Add_term_Click(object sender, EventArgs e)
        {
            string terms = Term1.Text;

            if (inputstr_add != "")
            {
                inputstr_add = inputstr_add + ",[" + terms + "]";
            }

            if (inputstr_add == "")
            {
                inputstr_add = "[" + terms + "]";
            }


            Input_text.Text = inputstr_add;

        }

        protected void Search_Click(object sender, EventArgs e)
        {
            String using_term1 = "art";
            String using_term2 = "good";
            String using_term3 = "true";
            String using_term4 = "false";
            String using_term5 = "cs";

            //--------------------------------------------------------------------------------------
            //object setting 
            List<string> termsone = new List<string>(new string[] { "dog", "pig", "cat" });
            string name1 = "cool";
            int counter1 = 0;
            List<string> termstwo = new List<string>(new string[] { "pig", "cat" });
            string name2 = "cool2";
            int counter2 = 2;
            List<string> termsthree = new List<string>(new string[] { "cat" });
            string name3 = "cool3";
            int counter3 = 3;

            List<objects_example> results = new List<objects_example>();

           // results.Add(new objects_example(name1, termsone, counter1));
            //results.Add(new objects_example(name2, termstwo, counter2));
            //results.Add(new objects_example(name3, termsthree, counter3));

            foreach (objects_example things in results)
            {
                ListBox1.Items.Add(things.name);
            }
            //----------------------------------------------------------------------------------------
            // show in the listbox
            ListBox1.Items.Add(using_term1);
            ListBox1.Items.Add(using_term2);
            ListBox1.Items.Add(using_term3);
            ListBox1.Items.Add(using_term4);
            ListBox1.Items.Add(using_term5);
        }

        protected void Sort_id_Click(object sender, EventArgs e)
        {
            ListBox2.Items.Clear();
            List<string> termsone = new List<string>(new string[] { "dog", "pig", "cat" });
            string name1 = "cool";
            int counter1 = 4;
            List<string> termstwo = new List<string>(new string[] { "pig", "cat" });
            string name2 = "cool2";
            int counter2 = 2;
            List<string> termsthree = new List<string>(new string[] { "cat" });
            string name3 = "cool3";
            int counter3 = 3;

            List<objects_example> results = new List<objects_example>();

           // results.Add(new objects_example(name1, termsone, counter1));
            //results.Add(new objects_example(name2, termstwo, counter2));
            //results.Add(new objects_example(name3, termsthree, counter3));

            //List<objects_example> sort_result = results.OrderBy< x=>x.results. >
            var sort_result = from element in results orderby element.counter select element;


            foreach (objects_example things in sort_result.Reverse().ToList())
            {
                ListBox2.Items.Add(things.name);
            }
        }

        protected void sort_11_Click(object sender, EventArgs e)
        {
            ListBox2.Items.Clear();
            List<string> termsone = new List<string>(new string[] { "dog", "pig", "cat" });
            string name1 = "Bcool";
            int counter1 = 4;
            List<string> termstwo = new List<string>(new string[] { "pig", "cat" });
            string name2 = "Acool2";
            int counter2 = 2;
            List<string> termsthree = new List<string>(new string[] { "cat" });
            string name3 = "Ccool3";
            int counter3 = 3;

            List<objects_example> results = new List<objects_example>();

            //results.Add(new objects_example(name1, termsone, counter1));
            //results.Add(new objects_example(name2, termstwo, counter2));
            //results.Add(new objects_example(name3, termsthree, counter3));

            var sort_result = from element in results orderby element.name select element;


            foreach (objects_example things in sort_result)
            {
                ListBox2.Items.Add(things.name);
            }
        }

        protected void get_button_Click(object sender, EventArgs e)
        {
            string input_str = "[pig],[dog],[cat]";
            string Triminput_str = input_str.Trim();
            string sstring = Triminput_str.Replace("[", "");
            sstring = sstring.Replace("]", "");
            List<string> new_str = sstring.Split(',').ToList();
            List<string> terms_two = new List<string>(new string[] { "pig", "cat" });
            int counter_test = 0;
            foreach (string items in new_str)
            {
                foreach (string thing in terms_two)
                {
                    if (items == thing)
                    {
                        counter_test = counter_test + 1;
                    }
                }
            }

            foreach (string things in new_str)
            {
                ListBox3.Items.Add(things);
            }
            ListBox3.Items.Add(counter_test.ToString());

        }

        protected void add2_Click(object sender, EventArgs e)
        {
            string terms = Label1.Text;
            if (inputstr_add != "")
            {
                inputstr_add = inputstr_add + ",[" + terms + "]";
            }

            if (inputstr_add == "")
            {
                inputstr_add = "[" + terms + "]";
            }


            Input_text.Text = inputstr_add;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var TestConn = new Neo4jDB();
            int id = 2;

            Classifiable classy = TestConn.getClassifiableById(id);
            string name;
            List<string> terms_term;
            int counter;
            string concept;
            string url;

            name = classy.name;
            url = classy.url;
            concept = classy.conceptStr.ToString();
            terms_term = classy.conceptStr.TolistString();
            ListBox4.Items.Add(name);
            ListBox4.Items.Add(url);
            ListBox4.Items.Add(concept);
            foreach (String things in terms_term)
            {
                ListBox4.Items.Add(things);
            }

        }
    }
   
    
    

}