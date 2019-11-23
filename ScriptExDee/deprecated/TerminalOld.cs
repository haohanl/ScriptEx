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
    static class TerminalOld
    {
        // Class variables
        public static List<string> userInput;
        public static List<string> userCommands;

        // Special characters
        public static string currentMode = "!s";
        public const string ThreadBreakKey = "|";
        public const string SoftwareModeKey = "!s";
        public const string TestModeKey = "!t";
        public const string QCModeKey = "!q";


        // Entrypoint for terminal
        public static void Start()
        {
            StartTerminalLoop();
        }

        // The main loop for terminal interactions
        static void StartTerminalLoop()
        {
            TitleScreen.ShowTitle();
            SoftwareModeTitle();
            
            do
            {
                ReadInput();
                if (ChangeModeState()) { continue; }
                switch (currentMode)
                {
                    case SoftwareModeKey:
                        SoftwareMode.RunCommands();
                        break;
                    case TestModeKey:
                        TestingMode.RunCommands();
                        break;
                    case QCModeKey:
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

            string input = Console.ReadLine();
            TerminalOld.userInput = new List<string>(input.Trim().Split(' '));
            userCommands = new List<string>(TerminalOld.userInput);
        }

        // Check for mode change
        static bool ChangeModeState()
        {
            string cmd = userInput[0];
            switch (cmd)
            {
                case SoftwareModeKey:
                    currentMode = SoftwareModeKey;
                    Console.Clear();
                    SoftwareModeTitle();
                    return true;
                case TestModeKey:
                    currentMode = TestModeKey;
                    Console.Clear();
                    TestModeTitle();
                    return true;
                case QCModeKey:
                    currentMode = QCModeKey;
                    Console.Clear();
                    QCModeRun();
                    return true;
                default:
                    break;
            }
            return false;
        }

        // Print program information and help
        static void SoftwareModeTitle()
        {
            string titleText = $"{Program.Title} [Build {Program.Version}]";
            Console.Title = titleText + " - SOFTWARE MODE";

            HRule();
            Console.WriteLine(titleText + " - " + Program.Quote);
            hRule();
            //Console.WriteLine($"CFG: \t'{Program.ConfigFile}'");
            Console.WriteLine($"SRC: \t'{AppConfig.SoftPath}'");
            Console.WriteLine($"DEST: \t'{AppConfig.SoftDestPath}'");
            hRule();
            SoftwareModeCommands();
            HRule();
            HelpText();
        }

        // Software mode commands
        static void SoftwareModeCommands()
        {
            // Print config commands
            int i = 0;
            foreach (var script in Program.Config.Scripts)
            {
                Console.WriteLine($" {script.Key} \t| {script.Desc} --- ['{Path.Combine(Program.Config.RoboCopy.SoftRoot, script.SourcePath)}']");

                i++;
                if (i == 4)
                {
                    hRule();
                }
            }
        }




        // Print program information and help
        static void TestModeTitle()
        {
            string titleText = $"{Program.Title} [Build {Program.Version}]";
            Console.Title = titleText + " - TESTING MODE";

            HRule();
            Console.WriteLine(titleText + " - " + Program.Quote);
            hRule();
            //Console.WriteLine($"CFG: \t'{Program.ConfigFile}'");
            Console.WriteLine($"SRC: \t'{AppConfig.TestPath}'");
            Console.WriteLine($"DEST: \t'{AppConfig.TestDestPath}'");
            hRule();
            TestModeCommands();
            HRule();
            TestModeStages();
            HRule();
            HelpText();
        }

        // Test mode commands
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
        }

        // Test mode stages
        static void TestModeStages()
        {
            // Print config stages
            foreach (var stage in Program.Config.TestStages)
            {
                Console.WriteLine($" {stage.Key} \t| {stage.Desc} --- ['{stage.Commands}']");
            }
        }




        // Run QCMode Sequence
        static void QCModeRun()
        {
            string titleText = $"{Program.Title} [Build {Program.Version}]";
            Console.Title = titleText + " - QC MODE";

            HRule();
            Console.WriteLine(titleText + " - " + Program.Quote);
            hRule();
            Console.WriteLine($"SRC: \t'{AppConfig.TestPath}'");
            Console.WriteLine($"DEST: \t'{AppConfig.TestDestPath}'");
            hRule();
            
            Console.WriteLine($"QC Mode Sequence:");

            TerminalOld.WriteLine("Run Windows Activation Dialog", "1");
            Thread QCWinAct = new Thread(QCMode.WinActivation);
            QCWinAct.Start();

            TerminalOld.WriteLine("Partitioning Disk Drives", "2");
            Thread QCDrives = new Thread(QCMode.FormatDrives);
            QCDrives.Start();

            hRule();

            QCDrives.Join();
            TerminalOld.WriteLine("Drives Partitioning Complete", "2");
            QCWinAct.Join();

            QCMode.ClearHeaven();
            QCMode.ClearSuperposition();
            TerminalOld.WriteLine("Superposition + Heaven folders cleared");

            hRule();
            TitleScreen.WriteSysSummary();
            Console.WriteLine();

            HRule();
            HelpText();
        }



        // Write one-time help text
        static void HelpText()
        {
            TerminalOld.WriteLine("'|' Threadblock, '!s' Software Mode, '!t' Testing Mode, '!q' QC Mode", "?");
        }

        public static void WriteLine(string outStr, string outChar = "-")
        {
            Console.WriteLine($"[{outChar}] {outStr}");
        }

        public static void hRule()
        {
            Rule('-');
        }
        public static void HRule()
        {
            Rule('=');
        }
        static void Rule(char c, int length = 80)
        {
            Console.WriteLine(new String(c, length));
        }
    }
}
