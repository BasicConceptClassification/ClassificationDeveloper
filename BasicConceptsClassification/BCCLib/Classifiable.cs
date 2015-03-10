using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class Classifiable
    {
        public enum Persmission
        {
            GLAM,
            OwnerOnly,
        };

        public enum Status
        {
            Classified,
            Unclassified,
        };

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

        /// <summary>
        /// To set/check permissions of a Classifiable.
        /// <para>
        /// Would like these to be enums once pulling that format from the DB is better?
        /// </para>
        /// </summary>
        public string perm
        {
            get;
            set;
        }

        /// <summary>
        /// Status of whether a Classifiable is classified or not.
        /// <para>
        /// Would like these to be enums once pulling that format from the DB is better?
        /// </para>
        /// </summary>
        public string status
        {
            get;
            set;
        }
    }
}
