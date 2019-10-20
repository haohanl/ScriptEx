using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ScriptEx
{
    static class Terminal
    {
        private const string ThreadBlockKey = "|";
        private const string QuitKey = "q";

        public static void Start()
        {
            WriteTitle();

            StartTerminalLoop();
        }

        static void StartTerminalLoop()
        {
            // Terminal loop vars
            string userInput;
            string[] rawCommands;
            List<Command> userCommands;
            bool quitNotDeclared = true;
            List<Thread> threadBatch;

            // Begin terminal loop
            do
            {
                // Read in user commands
                Console.Write("> ");
                userInput = Console.ReadLine();
                rawCommands = userInput.Trim().Split(' ');
                userCommands = ConvertRawCommands(rawCommands);

                // Process new line
                threadBatch = new List<Thread>();

                // RoboCopy Data
                Terminal.WriteLine("#", "Initialising Jobs.");
                foreach (Command command in userCommands)
                {
                    // check for special keys
                    switch (command.AttrVal)
                    {
                        // Thread blocking
                        case ThreadBlockKey:
                            break;
                        // Quit Key
                        case QuitKey:
                            break;
                        // Process command
                        default:
                            if (command.IsInvalidCommand())
                            {
                                Terminal.WriteLine("!", $"'{command.AttrVal}' is not a valid command. Ignoring.");
                                break;
                            }
                            RoboCopy.Run(command);
                            break;
                    }
                }

                HRule();
                // Install executables
                foreach (Command command in userCommands)
                {
                    // check for special keys
                    switch (command.AttrVal)
                    {
                        // Thread blocking
                        case ThreadBlockKey:
                            if (threadBatch.Count == 0)
                            {
                                Terminal.WriteLine("!", "No commands to thread batch. Ignoring thread batch command.");
                                break;
                            }

                            // Block thread batch
                            Terminal.WriteLine("-", $"Thread block initiated for '{threadBatch.Count}' commands.");
                            Program.ThreadBatchBlock(threadBatch);
                            threadBatch = new List<Thread>();
                            break;
                        // Quit Key
                        case QuitKey:
                            quitNotDeclared = false;
                            Terminal.WriteLine("-", "Quit key received. Program termination after command threads terminate.");
                            break;
                        // Process command
                        default:
                            if (command.IsInvalidCommand())
                            {
                                break;
                            }
                            threadBatch.Add(Installer.RunThread(command));
                            break;
                    }
                }

                // Wait for all running threads to terminate
                Program.ThreadBatchBlock(threadBatch);
                Terminal.WriteLine("#", "Command execution chain concluded.");
                Console.WriteLine();

            } while (quitNotDeclared);
        }

        static List<Command> ConvertRawCommands(string[] rawCommands)
        {
            List<Command> userCommands = new List<Command>();
            foreach (string command in rawCommands)
            {
                userCommands.Add(new Command(Program.AppConfig.GetCommand(command)));
            }

            return userCommands;
        }

        public static void WriteLine(string logChar, string log)
        {
            Console.WriteLine($"[{logChar}] [{CurrTime()}] {log}");
        }

        static void WriteTitle()
        {
            const string title = "ScriptEx";
            const string subtitle = "Written by HL, in his own damn time!";

            // Set Console title
            Console.Title = $"ScriptEx [Build {BuildNumber()}] - It's not a virus, I swear!";

            // Print header
            HRule('=');
            Console.WriteLine($"{title} [Build {BuildNumber()}] | {subtitle}");
            HRule();
            Console.WriteLine($"CFG: \t'{Program.ConfigFile}'");
            Console.WriteLine($"ROOT: \t'{Program.Root}'");
            Console.WriteLine($"SRC: \t'{Program.InstallDir}'");
            Console.WriteLine($"DEST: \t'{Program.CopyDestDir}'");
            HRule();
            PrintCommands();
            HRule('=');

        }

        static void PrintCommands()
        {
            Console.WriteLine("CONFIG LOADED - COMMANDS:");

            var commands = Program.AppConfig.GetKeys("ENTRY", "KEY");
            var descriptions = Program.AppConfig.GetKeys("ENTRY", "DESC");

            for (int i = 0; i < commands.Count(); i++)
            {
                Console.WriteLine($" {commands.ElementAt(i)} \t| {descriptions.ElementAt(i)} ('{Program.AppConfig.GetCommand(commands.ElementAt(i))[2]}')");
            }
        }

        static string BuildNumber()
        {
            return DateTime.Now.ToString("MMdd");
        }

        static string CurrTime()
        {
            return DateTime.Now.ToString("hh:mm:ss");
        }

        static void HRule(char c = '-', int length = 80)
        {
            Console.WriteLine(new String(c, length));
        }


    }

    public class Command
    {
        public string Exec;
        public string Args;
        public string SourcePath;
        public string DestPath;
        public bool IsThreadSafe;
        public string AttrVal;

        public string ExecPath;

        public Command(string[] commandData)
        {
            Exec = commandData[0];
            Args = commandData[1];
            SourcePath = commandData[2];
            DestPath = commandData[3];
            AttrVal = commandData[5];

            ExecPath = Program.CopyDestDir + DestPath + @"\" + Exec;
        }

        public bool IsInvalidCommand()
        {
            return String.IsNullOrEmpty(Exec) ? true : false;
        }
    }
}
