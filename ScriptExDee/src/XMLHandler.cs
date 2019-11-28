using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
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
            AppConfig config;
            

            // create serializer and file reader
            XmlSerializer serializer = new XmlSerializer(typeof(AppConfig));
            try
            {
                StreamReader reader = new StreamReader(xmlPath);

                // deserialize (read xml into class) xml file
                config = (AppConfig)serializer.Deserialize(reader);
                reader.Close();

                // return AppConfig
                return config;
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                Console.WriteLine("No configuration file found.\n Program will exit...");
                Console.ReadKey();
                Environment.Exit(-1);
            }

            return null;
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

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Test", IsNullable = false)]
        public AppConfigTest[] Tests { get; set; }

        // Retireve script based on key.
        public AppConfigTest GetTest(string key)
        {
            AppConfigTest tmp = null;

            foreach (var test in Tests)
            {
                if (test.Key == key)
                {
                    tmp = test;
                    break;
                }
            }

            return tmp;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Stage", IsNullable = false)]
        public AppConfigStage[] TestStages { get; set; }

        // Retireve script based on key.
        public AppConfigStage GetStage(string key)
        {
            AppConfigStage tmp = null;

            foreach (var test in TestStages)
            {
                if (test.Key == key)
                {
                    tmp = test;
                    break;
                }
            }

            return tmp;
        }


        // RoboCopy PATHs
        public static string SrcDriveLetter = null;
        public static string SoftPath = null;
        public static string SoftDestPath = null;
        public static string TestPath = null;
        public static string TestDestPath = null;

        // Find the source drive, as specified by the xml file
        public void InitialiseSrcDrive()
        {
            // Check to see if drive letter is forced
            if (RoboCopy.ForceSrcDrive != "")
            {
                SrcDriveLetter = RoboCopy.ForceSrcDrive + @":\";
                return;
            }

            // Search for matching volume
            foreach (LogicalDisk disk in SysInfo.LogicalDisks.List)
            {
                if (disk.Name == RoboCopy.SrcDrive)
                {
                    SrcDriveLetter = disk.DriveLetter + @"\";
                    return;
                }
            }

            // If nothing is found, assign default drive D:\
            SrcDriveLetter = @"D:\";
        }

        // Initialise the paths
        public void InitialisePaths()
        {
            SoftPath = Environment.ExpandEnvironmentVariables(SrcDriveLetter + RoboCopy.SoftRoot);
            SoftDestPath = Environment.ExpandEnvironmentVariables(RoboCopy.SoftDestRoot);
            TestPath = Environment.ExpandEnvironmentVariables(SrcDriveLetter + RoboCopy.TestRoot);
            TestDestPath = Environment.ExpandEnvironmentVariables(RoboCopy.TestDestRoot);
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
        public string SrcDrive { get; set; }

        /// <remarks/>
        public string ForceSrcDrive { get; set; }

        /// <remarks/>
        public string SoftRoot { get; set; }

        /// <remarks/>
        public string SoftDestRoot { get; set; }

        /// <remarks/>
        public string TestRoot { get; set; }

        /// <remarks/>
        public string TestDestRoot { get; set; }
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
            string root = Path.Combine(AppConfig.SoftPath, SourcePath);
            
            // Ensure the path exists
            try
            {
                path = new DirectoryInfo(root).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).Last().FullName;
            }
            catch (Exception)
            {
                Terminal.WriteLine($"Remote source does not exist for '{Key}'. Attempting local execution.", "!");
            }
            return path;
        }

        public string FullDestPath()
        {
            return Path.Combine(AppConfig.SoftDestPath, DestPath);
        }
    }

    /// <summary>
    /// Stores configuration of command and relevant installer.
    /// </summary>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class AppConfigTest
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
        public string DirPath { get; set; }

        /// <remarks/>
        public int Delay { get; set; }

        /// <remarks/>
        public bool IgnoreThreadBlock { get; set; }

        public string FullSourcePath()
        {
            string root = Path.Combine(AppConfig.SoftPath, DirPath);

            return root;
        }

        public string FullDestPath()
        {
            return Path.Combine(AppConfig.TestDestPath, DirPath);
        }
    }

    /// <summary>
    /// Stores configuration of test stages
    /// </summary>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class AppConfigStage
    {
        /// <remarks/>
        public string Key { get; set; }

        /// <remarks/>
        public string Desc { get; set; }

        /// <remarks/>
        public string Commands { get; set; }
    }
}
