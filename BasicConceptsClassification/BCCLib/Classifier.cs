using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class Classifier
    {
        /// <summary>
        /// Constructor for a Classifier. Requires the GLAM/organization and email of the Classifier. 
        /// Optional is the username.
        /// </summary>
        /// <param name="_organizationName">GLAM name.</param>
        /// <param name="_email">Email of the Classifier.</param>
        /// <param name="_username">Username of the Classifier.</param>
        public Classifier(GLAM _organizationName, string _email, string _username = "Unknown")
        {
            organization = _organizationName;
            email = _email;
            if (_username != null) username = _username;
        }

        /// <summary>
        /// The username that will be displayed to other Classifiers.
        /// </summary>
        public string username
        {
            get;
            set;
        }

        /// <summary>
        /// Email of the Classifier. Unique.
        /// </summary>
        public string email
        {
            get;
            set;
        }

        /// <summary>
        /// The GLAM organization that the Classifier belongs to.
        /// </summary>
        protected GLAM organization
        {
            get;
            set;
        }

        /// <summary>
        /// Get the name of the Organization.
        /// </summary>
        /// <returns></returns>
        public string getOrganizationName()
        {
            return organization.ToString();
        }

        /// <summary>
        /// Format: "USERNAME at GLAMNAME".
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return username + " at " + organization.ToString();
        }
    }
}
