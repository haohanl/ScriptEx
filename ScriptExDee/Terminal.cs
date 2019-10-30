using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ScriptExDee
{
    /// <summary>
    /// Run the output for the terminal. Contains methods for
    /// terminal manipulation.
    /// 
    /// Written by Haohan Liu
    /// </summary>
    static class Terminal
    {
        // Class variables
        private static string[] commandLine;
        private static List<string> commands;
        
        // Special characters
        private const string ThreadBreakKey = "|";


        // Entrypoint for terminal
        public static void Start()
        {
            WriteTitle();
            StartTerminalLoop();
        }

        // The main loop for terminal interactions
        static void StartTerminalLoop()
        {
            do
            {
                ReadInput();
                PrimeCommands();
                ExecuteScripts();
            } while (true);
        }

        // Read space delimited user input and process into list.
        static void ReadInput()
        {
            Console.Write("> ");

            string userInput = Console.ReadLine();
            commandLine = userInput.Trim().Split(' ');
            commands = new List<string>(commandLine);
        }

        // Filter commandline arguments and execute RoboCopy
        static void PrimeCommands()
        {
            // reset installCommands
            List<AppConfigScript> validCommands = new List<AppConfigScript>();

            // filter all commands
            foreach (string cmd in commandLine)
            {
                AppConfigScript tmp = Program.Config.GetScript(cmd);

                // Check for special characters
                switch (cmd)
                {
                    case ThreadBreakKey:
                        break;

                    // If none, check for commands
                    default:
                        switch (tmp)
                        {
                            // Remove invalid command from execution process
                            case null:
                                Terminal.WriteLine($"'{cmd}' is not a valid command. Ignoring.", "!");
                                commands.Remove(cmd);
                                break;
                            // Add valid command to execution process
                            default:
                                validCommands.Add(tmp);
                                break;
                        }
                        break;
                }
            }

            // RoboCopy valid commands
            RoboCopy.BatchCopy(validCommands);
        } 

        // execute script installers according to thread blocks
        static void ExecuteScripts()
        {
            List<Thread> threadBlock = new List<Thread>();

            foreach (var command in commands)
            {
                // Check for thread block
                if (command == ThreadBreakKey)
                {
                    // Trigger block
                    foreach (var thr in threadBlock)
                    {
                        thr.Join();
                    }
                    // Reset block
                    threadBlock = new List<Thread>();
                }
                else
                {
                    // Grab script and begin execution
                    AppConfigScript script = Program.Config.GetScript(command);
                    threadBlock.Add(Installer.StartInstall(script));
                }
            }

            // Trigger end of chain block
            foreach (var thr in threadBlock)
            {
                thr.Join();
            }
        }


        // Print program information and help
        static void WriteTitle()
        {
            string titleText = $"{Program.Title} [Build {Program.Version}] - {Program.Quote}";
            Console.Title = titleText;

            HRule();
            Console.WriteLine(titleText);
            hRule();
            Console.WriteLine($"CFG: \t'{Program.ConfigFile}'");
            Console.WriteLine($"ROOT: \t'{Program.RootPath}'");
            Console.WriteLine($"SRC: \t'{Program.SourcePath}'");
            Console.WriteLine($"DEST: \t'{Program.DestPath}'");
            hRule();

            // Print config commands
            foreach (var script in Program.Config.Scripts)
            {
                Console.WriteLine($" {script.Key} \t| {script.Desc} | '{script.FullSourcePath()}'");
            }

            HRule();

        }

        public static void WriteLine(string outStr, string outChar = "-")
        {
            Console.WriteLine($"[{outChar}] {outStr}");
        }

        static void hRule()
        {
            Rule('-');
        }
        static void HRule()
        {
            Rule('=');
        }
        static void Rule(char c, int length = 80)
        {
            Console.WriteLine(new String(c, length));
        }
    }
}
