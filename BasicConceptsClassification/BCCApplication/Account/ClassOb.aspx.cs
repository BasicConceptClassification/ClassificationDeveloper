﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCCLib;
using Neo4j;

namespace BCCApplication.Account
{
    public partial class ClassOb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SubmitObj_Click(object sender, EventArgs e)
        {
            string url = ObURL.Text;
            string name = ObName.Text;
            string concept = ObConcept.Text;

            Classifiable adding_classifiable = new Classifiable();

            //adding_classifiable.name = name;

            //ConceptString add_concept = new ConceptString();

            string Triminput_str = concept.Trim();
            string sstring = Triminput_str.Replace(")(", ",");
            sstring = sstring.Replace(")", "");
            sstring = sstring.Replace("(", "");
            List<string> new_str = sstring.Split(',').ToList();
           // List<string> result_needs = new List<string>();
          
            // foreach (string things in new_str)
           // {

           //     result_needs.Add("(" + things + ")");
           // }

          //  foreach (string things in result_needs)
          //  {
          //      ListBox1.Items.Add(things);
          //  }

            List<Term> new_terms = new List<Term>();

            foreach (String things in new_str)
            {
                //change to terms
                Term terterma = new Term { rawTerm = things, };
                //ListBox1.Items.Add(things);
                new_terms.Add(terterma);
            }

            ConceptString add_concept = new ConceptString
            {
                terms = new_terms,

            };

            adding_classifiable.name = name;
            adding_classifiable.conceptStr = add_concept;
            adding_classifiable.url = url;
            adding_classifiable.perm = "None";
            //adding_classifiable.owner = "Temp";


            //adding_classifiable.conceptStr = concept;

        }


    }
}