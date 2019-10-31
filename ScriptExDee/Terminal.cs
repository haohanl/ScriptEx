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
        private static string currentMode = "!s";
        private const string ThreadBreakKey = "|";
        private const string SoftwareModeKey = "!s";
        private const string TestModeKey = "!b";


        // Entrypoint for terminal
        public static void Start()
        {
            BlinkStickHandler.Start();
            StartTerminalLoop();
        }

        // The main loop for terminal interactions
        static void StartTerminalLoop()
        {
            BlinkStickHandler.SetState("def");
            SoftwareModeTitle();
            SoftwareModeCommands();
            HelpText();
            do
            {
                switch (currentMode)
                {
                    case SoftwareModeKey:
                        BlinkStickHandler.SetState("idle");
                        ReadInput();
                        if (ChangeModeState())
                        {
                            break;
                        }
                        BlinkStickHandler.SetState("installing");
                        PrimeSoftwareCommands();
                        BlinkStickHandler.SetState("success");
                        ExecuteSoftwareScripts();
                        BlinkStickHandler.SetState("def");
                        break;
                    case TestModeKey:
                        BlinkStickHandler.SetState("idle");
                        ReadInput();
                        if (ChangeModeState())
                        {
                            break;
                        }
                        BlinkStickHandler.SetState("testing");
                        PrimeTestingCommands();
                        ExecuteTests();
                        BlinkStickHandler.SetState("def");
                        break;
                    default:
                        break;
                }
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

        // Check for mode change
        static bool ChangeModeState()
        {
            string cmd = commandLine[0];
            switch (cmd)
            {
                case SoftwareModeKey:
                    currentMode = SoftwareModeKey;
                    Console.Clear();
                    SoftwareModeTitle();
                    SoftwareModeCommands();
                    HelpText();
                    return true;
                case TestModeKey:
                    currentMode = TestModeKey;
                    Console.Clear();
                    TestModeTitle();
                    TestModeCommands();
                    HelpText();
                    BlinkStickHandler.SetState("installing");
                    TransferTestingFolder();
                    BlinkStickHandler.SetState("idle");
                    return true;
                default:
                    break;
            }
            return false;
        }

        
        // Filter commandline arguments and execute RoboCopy
        static void PrimeSoftwareCommands()
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
        static void ExecuteSoftwareScripts()
        {
            List<Thread> threadBlock = new List<Thread>();

            foreach (var command in commands)
            {
                // Check for thread block
                if (command == ThreadBreakKey)
                {
                    // Terminal Output
                    Terminal.WriteLine("Start thread batch.", "|");

                    // Trigger block
                    foreach (var thr in threadBlock)
                    {
                        thr.Join();
                    }
                    // Reset block
                    threadBlock = new List<Thread>();
                    // Terminal Output
                    Terminal.WriteLine("Ended thread batch.", "|");
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
        static void SoftwareModeTitle()
        {
            string titleText = $"{Program.Title} [Build {Program.Version}] - SOFTWARE INSTALLATION MODE";
            Console.Title = titleText;

            HRule();
            Console.WriteLine(titleText);
            hRule();
            Console.WriteLine($"CFG: \t'{Program.ConfigFile}'");
            Console.WriteLine($"ROOT: \t'{Program.RootPath}'");
            //Console.WriteLine($"SRC: \t'{Program.SourcePath}'");
            Console.WriteLine($"DEST: \t'{Program.DestPath}'");
            hRule();
        }

        // Software mode commands
        static void SoftwareModeCommands()
        {
            // Print config commands
            int i = 0;
            foreach (var script in Program.Config.Scripts)
            {
                Console.WriteLine($" {script.Key} \t| {script.Desc} --- ['{Path.Combine(Program.Config.RoboCopy.SourceRoot, script.SourcePath)}']");

                i++;
                if (i == 4)
                {
                    hRule();
                }
            }

            HRule();
        }


        // Filter commandline arguments and execute RoboCopy
        static void PrimeTestingCommands()
        {
            // filter all commands
            foreach (string cmd in commandLine)
            {
                AppConfigTest tmp = Program.Config.GetTest(cmd);

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
                                break;
                        }
                        break;
                }
            }
        }

        // execute script installers according to thread blocks
        static void ExecuteTests()
        {
            List<Thread> threadBlock = new List<Thread>();

            foreach (var command in commands)
            {
                // Check for thread block
                if (command == ThreadBreakKey)
                {
                    // Terminal Output
                    Terminal.WriteLine("Start thread batch.", "|");

                    // Trigger block
                    foreach (var thr in threadBlock)
                    {
                        thr.Join();
                    }
                    // Reset block
                    threadBlock = new List<Thread>();
                    // Terminal Output
                    Terminal.WriteLine("Ended thread batch.", "|");
                }
                else
                {
                    // Grab script and begin execution
                    AppConfigTest test = Program.Config.GetTest(command);
                    threadBlock.Add(Testing.StartTest(test));
                }
            }

            // Trigger end of chain block
            foreach (var thr in threadBlock)
            {
                thr.Join();
            }
        }

        // Print program information and help
        static void TestModeTitle()
        {
            string titleText = $"{Program.Title} [Build {Program.Version}] - TESTING MODE - ";
            Console.Title = titleText;

            HRule();
            Console.WriteLine(titleText);
            hRule();
            Console.WriteLine($"CFG: \t'{Program.ConfigFile}'");
            Console.WriteLine($"ROOT: \t'{Path.Combine(Program.DriveLetter, Program.Config.RoboCopy.TestRoot)}'");
            //Console.WriteLine($"SRC: \t'{Program.SourcePath}'");
            Console.WriteLine($"DEST: \t'{Program.TestDestPath}'");
            hRule();
        }

        // Software mode commands
        static void TestModeCommands()
        {
            // Print config commands
            int i = 0;
            foreach (var test in Program.Config.Tests)
            {
                Console.WriteLine($" {test.Key} \t| {test.Desc} --- ['{Path.Combine(Program.Config.RoboCopy.TestRoot, test.DirPath)}']");

                i++;
                if (i == 5)
                {
                    hRule();
                }
            }

            HRule();
        }

        static void TransferTestingFolder()
        {
            RoboCopy.Copy();
        }




        // Write one-time help text
        static void HelpText()
        {
            Console.WriteLine("'|' Threadblock, '!s' Software Mode, '!b' Benchmark Mode");
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
