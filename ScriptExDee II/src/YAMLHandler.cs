using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YamlDotNet.Serialization;

namespace ScriptExDee_II
{
    static class YAMLHandler
    {
        public static Config Deserialize(string filePath)
        {
            try
            {
                var _input = new StreamReader(filePath);

                var _deserializer = new DeserializerBuilder().Build();

                var _config = _deserializer.Deserialize<Config>(_input);

                return _config;
            }
            catch (Exception ex)
            {
                throw new IOException($"Unable to deserialize '{filePath}'.", ex);
            }
        }
    }


    /// <summary>
    /// Root object structure for YAML input
    /// </summary>
    public class Config
    {
        public ProgramConfig Program { get; set; }
        public RoboCopyConfig RoboCopy { get; set; }
        public Dictionary<string, ModeConfig> Modes { get; set; }
        public Dictionary<string, MacroItem> Macros { get; set; }

        // Validation Methods
        public bool IsModeKey(string key)
        {
            return Program.ModeKeys.ContainsKey(key);
        }
        
        public bool IsSpecialKey(string key)
        {
            return Program.SpecialKeys.ContainsKey(key);
        }

        public bool IsThreadBlock(string key)
        {
            if (GetSpecialKey(key) == "Threadblock")
            {
                return true;
            }
            return false;
        }

        public bool IsHelpKey(string key)
        {
            if (GetSpecialKey(key) == "Show Commands")
            {
                return true;
            }
            return false;
        }

        public bool IsMacroKey(string key)
        {
            return Macros.ContainsKey(key);
        }

        public bool ContainsKey(string mode, string key)
        {
            if (Modes.ContainsKey(mode))
            {
                if (Modes[mode].Commands.ContainsKey(key))
                {
                    return true;
                }
            }

            return false;
        }

        // Retrieval Methods
        public string GetMode(string mode)
        {
            if (IsModeKey(mode))
            {
                return Program.ModeKeys[mode];
            }
            return null;
        }

        public string GetSpecialKey(string key)
        {
            if (IsSpecialKey(key))
            {
                return Program.SpecialKeys[key];
            }
            return null;
        }

        public MacroItem GetMacroItem(string key)
        {
            if (IsMacroKey(key))
            {
                return Macros[key];
            }
            return null;
        }

        public CommandItem GetCommandItem(string mode, string key)
        {
            if (ContainsKey(mode, key))
            {
                return Modes[mode].Commands[key];
            }

            return null;
        }
    }

    public class ProgramConfig
    {
        public bool DisableSystemCheck { get; set; }
        public bool AutoWinUpdate { get; set; }
        public bool IgnoreInvalidCommands { get; set; }
        public bool SetPerformanceMode { get; set; }
        public int TitleScreenDelay { get; set; }
        public string DefaultMode { get; set; }
        public Dictionary<string, string> ModeKeys { get; set; }
        public Dictionary<string, string> SpecialKeys { get; set; }
    }

    public class RoboCopyConfig
    {
        public string SrcDriveName { get; set; }
        public string SrcDriveRoot { get; set; }
        public DriveType SrcDriveType { get; set; }
        public bool ForceSrcDriveLetter { get; set; }
        public string SrcDriveLetter { get; set; }
        public bool SrcUseNetwork { get; set; }
        public bool SrcPreferNetwork { get; set; }
        public string SrcNetworkRoot { get; set; }
    }

    public class ModeConfig
    {
        public bool SrcCopy { get; set; }
        public string SrcModeRoot { get; set; }
        public string DstModeRoot { get; set; }
        public List<string> Categories { get; set; }
        public Dictionary<string, CommandItem> Commands { get; set; }

        /// <summary>
        /// Return a List of CommandItem objects that match the given category
        /// </summary>
        public List<CommandItem> GetCategory(string category)
        {
            // search for matching items
            var list = new List<CommandItem>();

            foreach (var command in Commands.Values)
            {
                if (command.Category == category)
                {
                    list.Append(command);
                }
            }

            return list;
        }
    }

    public class CommandItem
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Exec { get; set; }
        public string Args { get; set; }
        public string Path { get; set; }
        public int Delay { get; set; }
        public bool IgnoreThreadBlock { get; set; }
        public override string ToString()
        {
            return $"Name: {Name}\nCategory: {Category}\nExec: {Exec}\nArgs: {Args}\nPath: {Path}\nDelay: {Delay}\nIgnoreBlockThread: {IgnoreThreadBlock}";
        }

        /// <summary>
        /// Return the path of the newest folder in the command root
        /// </summary>
        public string GetNewestSrcPath(string modeRoot)
        {
            // Path variables           
            string _srcRoot = System.IO.Path.Combine(modeRoot, Path);
            string _srcPath = null;

            // Ensure the path exists
            try
            {
                _srcPath = new DirectoryInfo(_srcRoot).GetDirectories().OrderBy(fi => fi.CreationTime).Last().FullName;
            }
            catch (Exception)
            {
                Terminal.WriteLine($"Remote source does not exist for '{Name}'. Attempting local execution.", "!");
                return null;
            }
            return _srcPath;
        }

        /// <summary>
        /// Return the path of the destination command root
        /// </summary>
        public string GetDstPath(string modeRoot)
        {
            string _dstRoot = Environment.ExpandEnvironmentVariables(modeRoot);
            string _dstPath = System.IO.Path.Combine(_dstRoot, Path);
            return _dstPath;
        }
    }

    public class MacroItem
    {
        public string Name { get; set; }
        public string SetMode { get; set; }
        public string Command { get; set; }
        public override string ToString()
        {
            return $"Name: {Name}\nSetMode: {SetMode}\nCommand: {Command}";
        }
    }



    [Serializable]
    public class InvalidMacroException : Exception
    {
        public InvalidMacroException() { }
        public InvalidMacroException(string message) : base(message) { }
        public InvalidMacroException(string message, Exception inner) : base(message, inner) { }
        protected InvalidMacroException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
