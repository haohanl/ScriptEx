﻿using System;
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
        public static List<string> userInput;
        public static List<string> userCommands;

        // Special characters
        public static string currentMode = "!s";
        public const string ThreadBreakKey = "|";
        public const string SoftwareModeKey = "!s";
        public const string TestModeKey = "!t";


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
            Terminal.userInput = new List<string>(input.Trim().Split(' '));
            userCommands = new List<string>(Terminal.userInput);
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
                    TransferTestingFolder();
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
            Console.WriteLine($"CFG: \t'{Program.ConfigFile}'");
            Console.WriteLine($"SRC: \t'{Program.SoftPath}'");
            Console.WriteLine($"DEST: \t'{Program.SoftDestPath}'");
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
            Console.WriteLine($"CFG: \t'{Program.ConfigFile}'");
            Console.WriteLine($"SRC: \t'{Program.TestPath}'");
            //Console.WriteLine($"SRC: \t'{Program.SourcePath}'");
            Console.WriteLine($"DEST: \t'{Program.TestDestPath}'");
            hRule();
            TestModeCommands();
            HRule();
            TestModeStages();
            HRule();
            HelpText();
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
        }

        // Software mode stages
        static void TestModeStages()
        {
            // Print config stages
            foreach (var stage in Program.Config.TestStages)
            {
                Console.WriteLine($" {stage.Key} \t| {stage.Desc} --- ['{stage.Commands}']");
            }
        }

        static void TransferTestingFolder()
        {
            RoboCopy.CopyTesting();
        }


        

        // Write one-time help text
        static void HelpText()
        {
            Terminal.WriteLine("'|' Threadblock, '!s' Software Mode, '!t' Testing Mode", "?");
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
