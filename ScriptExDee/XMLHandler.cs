using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ScriptExDee
{
    /// <summary>
    /// Intake XML configuration file and convert into program usable form.
    /// 
    /// Written by Haohan Liu
    /// </summary>
    static class XMLHandler
    {
        /// <summary>
        /// Intake XML file and deserialise.
        /// 
        /// Code adapted from: https://stackoverflow.com/questions/364253/how-to-deserialize-xml-document
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public static AppConfig Initialise(string xmlPath)
        {
            // create return var
            AppConfig config = null;

            // create serializer and file reader
            XmlSerializer serializer = new XmlSerializer(typeof(AppConfig));
            StreamReader reader = new StreamReader(Program.ConfigFile);

            // deserialize (read xml into class) xml file
            config = (AppConfig)serializer.Deserialize(reader);
            reader.Close();

            // return AppConfig
            return config;
        }
    }


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    // NOTE: https://stackoverflow.com/questions/364253/how-to-deserialize-xml-document
    /// <summary>
    /// Master class to store config information.
    /// </summary>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class AppConfig
    {
        /// <remarks/>
        public AppConfigRoboCopy RoboCopy { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Script", IsNullable = false)]
        public AppConfigScript[] Scripts { get; set; }

        // Retireve script based on key.
        public AppConfigScript GetScript(string key)
        {
            AppConfigScript tmp = null;

            foreach (var script in Scripts)
            {
                if (script.Key == key)
                {
                    tmp = script;
                    break;
                }
            }

            return tmp;
        }
    }

    /// <summary>
    /// Stores source and destination roots of file transfers
    /// </summary>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class AppConfigRoboCopy
    {
        /// <remarks/>
        public string SourceRoot { get; set; }

        /// <remarks/>
        public string DestRoot { get; set; }
    }

    /// <summary>
    /// Stores configuration of command and relevant installer.
    /// </summary>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class AppConfigScript
    {
        /// <remarks/>
        public string Key { get; set; }

        /// <remarks/>
        public string Desc { get; set; }

        /// <remarks/>
        public string Exec { get; set; }

        /// <remarks/>
        public string Args { get; set; }

        /// <remarks/>
        public string Auto { get; set; }

        /// <remarks/>
        public string SourcePath { get; set; }

        /// <remarks/>
        public string DestPath { get; set; }

        /// <remarks/>
        public bool Threadsafe { get; set; }

        public string FullSourcePath()
        {
            string path = null;
            string root = Path.Combine(Program.SourcePath, SourcePath);
            
            // Ensure the path exists
            try
            {
                path = new DirectoryInfo(root).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).Last().FullName;
            }
            catch (Exception)
            {
                Terminal.WriteLine($"'{Key}' source path not valid in '{Program.ConfigFile}'. Continue at your own risk.", "!");
                Console.ReadKey();
            }
            return path;
        }

        public string FullDestPath()
        {
            return Path.Combine(Program.DestPath, DestPath);
        }
    }
}
