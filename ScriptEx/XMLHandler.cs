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

            // form command
            IEnumerable<string> command = from key in xml.Descendants(elem)
                                          where (string)key.Attribute(attr) == attrVal
                                          from cmd in key.Descendants(exec)
                                          select (string)cmd;

            // form args
            IEnumerable<string> arguments = from key in xml.Descendants(elem)
                                            where (string)key.Attribute(attr) == attrVal
                                            from arg in key.Descendants(args)
                                            select (string)arg;

            // form return
            string[] ret = { string.Join(" ", command), string.Join(" ", arguments) };
            return ret;
        }
    }
}
