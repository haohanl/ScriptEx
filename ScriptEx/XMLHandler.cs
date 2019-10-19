using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScriptEx
{
    /// <summary>
    /// To intake an XML configuration of shell commands and provide a searchable link
    /// between a key and its executable and parameters.
    /// </summary>
    class XMLHandler
    {
        private readonly XDocument xml;

        public XMLHandler(string file)
        {
            xml = XDocument.Load(file);
        }

        public string[] Get(string attrVal)
        {
            // constants because laziness
            const string attr = "KEY";
            const string elem = "ENTRY";

            const string exec = "EXEC";
            const string args = "ARGS";
            const string path = "PATH";
            const string auto = "AUTO";
            const string thread = "THREADSAFE";

            string[] searchArray = { exec, args, path, auto, thread };
            string[] returnArray = new string[searchArray.Length];

            int i = 0;
            foreach (var item in searchArray)
            {
                IEnumerable<string> val = from key in xml.Descendants(elem)
                                          where (string)key.Attribute(attr) == attrVal
                                          from v in key.Descendants(item)
                                          select (string)v;

                returnArray[i] = string.Join(" ", val);
                i++;
            }

            return returnArray;
        }
    }
}
