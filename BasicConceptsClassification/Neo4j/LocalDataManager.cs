using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Neo4j
{
    public class LocalDataManager
    {
        public enum BCCContentFile
        {
            About
        }

        public static void Save(string msg, BCCContentFile what)
        {
            validatePath(what);

            IEnumerable<string> package = new string[] { msg };

            File.WriteAllLines(getPath(what), package, Encoding.UTF8);
        }

        public static string Load(BCCContentFile what)
        {
            validatePath(what);

            string[] raw = File.ReadAllLines(getPath(what), Encoding.UTF8);

            StringBuilder sb = new StringBuilder();
            foreach (string s in raw)
            {
                sb.Append(s);
            }

            return sb.ToString();
        }

        protected static string getPath(BCCContentFile what)
        {
            switch (what)
            {
                case BCCContentFile.About:
                    return "about.txt";

                default:
                    return "text.txt";
            }
        }

        protected static void validatePath(BCCContentFile what)
        {
            string path = getPath(what);
            var file = new FileStream(getPath(what), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            file.Close();
        }
    }
}
