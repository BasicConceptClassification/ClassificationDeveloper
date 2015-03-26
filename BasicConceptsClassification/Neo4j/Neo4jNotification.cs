using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4j
{
    class Neo4jNotification
    {
        public String msg
        {
            get;
            set;
        }

        // As UNIX timestamp.
        public long time
        {
            get;
            set;
        }
    }
}
