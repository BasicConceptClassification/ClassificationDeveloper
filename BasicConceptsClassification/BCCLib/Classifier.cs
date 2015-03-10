using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class Classifier
    {
        public Classifier(GLAM _organization)
        {
            organization = _organization;
        }

        public String name
        {
            get;
            set;
        }

        public string email
        {
            get;
            set;
        }

        protected GLAM organization
        {
            get;
            set;
        }

        public string getOrganizationName()
        {
            return organization.ToString();
        }

        public override string ToString()
        {
            return name + " " + organization.ToString();
        }
    }
}
