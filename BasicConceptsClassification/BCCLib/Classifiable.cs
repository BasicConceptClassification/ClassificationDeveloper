using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class Classifiable
    {
        /// <summary>
        /// A set of permissions to describe which Classifiers can edit the classifiable.
        /// GLAM means any Classifiers who are in the same GLAM as the owner can edit it.
        /// OnlyOnly means that only the owner can edit the Classifiable.
        /// </summary>
        public enum Permission
        {
            GLAM,
            OwnerOnly,
        };

        /// <summary>
        /// A set of statuses to describe the state of the classifiable.
        /// Classified means if there is a Concept String and nothing is 'wrong' with it.
        /// Unclassified means if there is no Concept String.
        /// AdminModified means that there may or may not be a Concept String, but the
        /// admin may have modified it in some way.
        /// </summary>
        public enum Status
        {
            Classified,
            Unclassified,
            AdminModified,
        };

        /// <summary>
        /// A unique Id of a Classifiable. Format is [GlamOfOwner]_[ClassifiableName].
        /// </summary>
        public string id
        {
            get;
            set;
        }

        /// <summary>
        /// Name of a Classifiable. Must be unique to the GLAM it is in.
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
        /// The Classifier that has added the Classifiable.
        /// </summary>
        public Classifier owner
        {
            get;
            set;
        }

        /// <summary>
        /// The Classifier who last editied the Classifiable.
        /// </summary>
        public Classifier classifierLastEdited
        {
            get;
            set;
        }

        /// <summary>
        /// Permissions of a Classifiable. Either only the one who added the classifiable
        /// can edit it or anyone in that Classifiable's GLAM can edit it.
        /// </summary>
        public string perm
        {
            get;
            set;
        }

        /// <summary>
        /// Status of whether a Classifiable is classified, not classified, or has been modified
        /// by the admin.
        /// </summary>
        public string status
        {
            get;
            set;
        }
    }
}
