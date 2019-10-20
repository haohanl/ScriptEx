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
        private static string CommandFile = "cmd.xml";
        private static XMLHandler TerminalCommands = new XMLHandler(CommandFile);

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
            string[] userCommands;
            Command userCommand;
            bool quitNotDeclared = true;
            List<Thread> threadBatch;

            // Begin terminal loop
            do
            {
                // Read in user commands
                userInput = Console.ReadLine();
                userCommands = userInput.Trim().Split(' ');

                // Process new line
                threadBatch = new List<Thread>();

                // Process each command
                foreach (string command in userCommands)
                {
                    // check for special keys
                    switch (command)
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
                            foreach (Thread thread in threadBatch)
                            {
                                thread.Join();
                            }
                            threadBatch = new List<Thread>();
                            break;
                        // Quit Key
                        case QuitKey:
                            quitNotDeclared = false;
                            Terminal.WriteLine("-", "Quit key received. Program termination after command threads terminate.");
                            break;
                        // Process command
                        default:
                            userCommand = new Command(TerminalCommands.Get(command));
                            if (userCommand.IsInvalidCommand())
                            {
                                Terminal.WriteLine("!", $"'{command}' is not a valid command. Ignoring.");
                                break;
                            }
                            threadBatch.Add(Installer.RunThread(userCommand.ExecPath, userCommand.Args));
                            break;
                    }
                }

            } while (quitNotDeclared);
        }

        public static void WriteLine(string logChar, string log)
        {
            Console.WriteLine($"[{logChar}] [{CurrTime()}] {log}");
        }

        static void WriteTitle()
        {
            const string title = "ScriptEx";
            const string subtitle = "Don't worry, it's not sketchy at all.";

            HRule('=');
            Console.WriteLine($"{title} [Build {BuildNumber()}] | {subtitle}");
            HRule();
            Console.WriteLine($"ROOT: {GetExecRoot()}");
            HRule('=');

        }

        static string GetExecRoot()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
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

    class Command
    {
        public string Exec;
        public string Args;
        public string Auto;
        public string Path;

        public string ExecPath;

        public Command(string[] commandData)
        {
            Exec = commandData[0];
            Args = commandData[1];
            Auto = commandData[2];
            Path = commandData[3];

            ExecPath = Path + Exec;
        }

        public bool IsInvalidCommand()
        {
            return String.IsNullOrEmpty(Exec) ? true : false;
        }
    }
}
