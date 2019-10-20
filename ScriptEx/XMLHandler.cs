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
    /// 
    /// 
    /// 
    /// Written by Haohan Liu, in his own damn time.
    /// </summary>
    class XMLHandler
    {
        private readonly XDocument xml;

        public XMLHandler(string file)
        {
            try
            {
                xml = XDocument.Load(file);
            }
            catch (Exception)
            {
                Console.WriteLine($"'{Program.ConfigFile}' not found. Program will terminate.");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        public string[] GetCommand(string attrVal)
        {
            // constants because laziness
            const string attr = "KEY";
            const string elem = "ENTRY";

            const string exec = "EXEC";
            const string args = "ARGS";
            const string spath = "SOURCEPATH";
            const string dpath = "DESTPATH";
            const string thread = "THREADSAFE";

            string[] searchArray = { exec, args, spath, dpath, thread };
            string[] returnArray = new string[searchArray.Length + 1];

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

            returnArray[i] = attrVal;
            return returnArray;
        }

        public string GetNode(string node)
        {
            IEnumerable<string> val = from key in xml.Descendants(node)
                                      select (string)key;
            
            return string.Join(" ", val).Trim();
        }

        public IEnumerable<string> GetKeys(string node, string key)
        {
            IEnumerable<string> val = from n in xml.Descendants(node)
                                      select (string)n.Attribute(key);
            return val;
        }
    }
}
