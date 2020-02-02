using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using YamlDotNet.Serialization;

namespace YAMLReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var _config = YAMLDeserializer.Deserialize("tst.yml");

            Console.WriteLine(String.Join("\n\n", _config.Testing.Commands));


            Console.WriteLine("Program has ended...");
            Console.ReadKey();
        }
    }

    static class YAMLDeserializer
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
                Console.WriteLine($"Unable to deserialize '{filePath}'. See error below:");
                Console.WriteLine(ex);
                Console.ReadKey();
                Environment.Exit(-1);
            }

            return null;
        }
    }

    public class Config
    {
        public ProgramConfig Program { get; set; }
        public RoboCopyConfig RoboCopy { get; set; }
        public ModeConfig Software { get; set; }
        public ModeConfig Testing { get; set; }
        public List<MacroItem> Macros { get; set; }
    }

    public class ProgramConfig
    {
        public bool LogOutput { get; set; }
        public bool DisableSystemCheck { get; set; }
        public bool AutoWinUpdate { get; set; }
    }

    public class RoboCopyConfig
    {
        public string SrcDriveName { get; set; }
        public string SrcDriveType { get; set; }
        public string SrcDriveRoot { get; set; }
        public string DstDriveRoot { get; set; }
    }

    public class ModeConfig
    {
        public string ModeKey { get; set; }
        public List<string> Categories { get; set; }
        public List<CommandItem> Commands { get; set; }

        /// <summary>
        /// Return a CommandItem given a matching command key.
        /// </summary>
        public CommandItem GetCommand(string commandKey)
        {
            foreach (var command in Commands)
            {
                if (command.Key == commandKey)
                {
                    return command;
                }
            }

            return null;
        }
    }

    public class CommandItem
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Key { get; set; }
        public string Exec { get; set; }
        public string Args { get; set; }
        public string Path { get; set; }
        public int Delay { get; set; }
        public bool IgnoreThreadBlock { get; set; }
        public override string ToString()
        {
            return $"Name: {Name}\nCategory: {Category}\nKey: {Key}\nExec: {Exec}\nArgs: {Args}\nPath: {Path}\nDelay: {Delay}\nIgnoreBlockThread: {IgnoreThreadBlock}";
        }
    }

    public class MacroItem
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string SetMode { get; set; }
        public string Command { get; set; }
        public override string ToString()
        {
            return $"Name: {Name}\nKey: {Key}\nSetMode: {SetMode}\nCommand: {Command}";
        }
    }
}
