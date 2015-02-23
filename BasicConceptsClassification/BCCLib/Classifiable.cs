using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class Classifiable
    {
        /// <summary>
        /// A unique Id of a Classifiable.
        /// </summary>
        public string id
        {
            get;
            set;
        }

        /// <summary>
        /// Name of a Classifiable. Not unique.
        /// </summary>
        public string name
        {
            get;
            set;
        }

        /// <summary>
        /// The URL to the GLAM that has this Classifiable.
        /// </summary>
        public string url
        {
            get;
            set;
        }

        /// <summary>
        /// A series of Terms that are used to classify the Classifiable.
        /// </summary>
        public ConceptString conceptStr
        {
            get;
            set;
        }

        /// <summary>
        /// A Classifier that has added the Classifiable.
        /// </summary>
        public Classifier owner
        {
            get;
            set;
        }
    }
}
