using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ScriptExDee_II
{
    public static class Terminal
    {

        // Terminal static variables
        static string CurrentMode = "Software";
        static bool IsCommandClean;
        static List<string> CommandList;

        public static void Start()
        {
            while (true)
            {
                // Reset lists
                CommandList = ProcessUserInput();
                TransferSoftware(CommandList, CurrentMode);

                Console.Write("Commands: ");
                foreach (string command in CommandList)
                {
                    Console.Write(command + " ");
                }
                Console.WriteLine();
            }
        }


        /// <summary>
        /// Intake user input for validation and conversion into CommandItems
        /// </summary>
        static List<string> ProcessUserInput()
        {
            // Preprocessing variables
            string _input;
            List<string> _commands;
            List<string> _validCommands;

            // Read user input
            do
            {
                IsCommandClean = true;
                _input = Console.ReadLine();
                _commands = new List<string>(_input.Trim().Split(' '));
                _validCommands = ValidateCommands(_commands, CurrentMode);

            } while (!IsCommandClean && !Program.Config.Program.IgnoreInvalidCommands);

            return _validCommands;
        }

        /// <summary>
        /// Validate user commands and macros with recursive validation of all macro command items. 
        /// </summary>
        /// <param name="commands">List of command key strings</param>
        /// <param name="currentMode">Current operating mode of program</param>
        /// <returns>List of validated commands</returns>
        static List<string> ValidateCommands(List<string> commands, string currentMode)
        {
            string _currentMode;
            List<string> _validCommands;

            // Prepare new check
            _validCommands = new List<string>();
            _currentMode = currentMode;


            // Validate user input, continue once input is clean or ignoring invalid inputs
            foreach (var key in commands)
            {
                // Check for macro keys
                if (Program.Config.IsMacroKey(key))
                {
                    // Expand macro into commands
                    MacroItem _macro = Program.Config.GetMacroItem(key);
                    List<string> _expandedMacro = new List<string>(_macro.Command.Trim().Split(' '));
                    string _macroMode = _macro.SetMode;

                    // Check for potential stack overflow
                    if (_expandedMacro.Contains(key))
                    {
                        throw new InvalidMacroException("Macro command cannot call itself.");
                    }

                    // Macro operating mode validation
                    if (_currentMode != Program.Config.GetMode(_macroMode))
                    {
                        _currentMode = Program.Config.GetMode(_macroMode);
                        _validCommands.Add(_macroMode);
                    }

                    // Validate macro command items
                    _validCommands.AddRange(ValidateCommands(_expandedMacro, _currentMode));

                    continue;
                }

                // Check for mode keys
                if (Program.Config.IsModeKey(key))
                {
                    _currentMode = Program.Config.GetMode(key);
                    _validCommands.Add(key);
                    continue;
                }

                // Check for special keys
                if (Program.Config.IsSpecialKey(key))
                {
                    _validCommands.Add(key);
                    continue;
                }

                // Check for key validity
                if (Program.Config.ContainsKey(_currentMode, key))
                {
                    _validCommands.Add(key);
                    continue;
                }

                // If a command reaches this point, it is not valid
                WriteLine($"'{key}' is not a valid command in '{_currentMode} Mode'.", "!");
                IsCommandClean = false;
            }

            // Return validated list
            return _validCommands;
        }
        
        
        
        /// <summary>
        /// Transfer all required software from remote source
        /// </summary>
        static void TransferSoftware(List<string> commands, string currentMode)
        {
            string _currentMode;

            // Prepare new check
            List<Thread> threadBatch = new List<Thread>();
            _currentMode = currentMode;


            // Validate user input, continue once input is clean or ignoring invalid inputs
            foreach (var key in commands)
            {

                // Check for mode keys
                if (Program.Config.IsModeKey(key))
                {
                    _currentMode = Program.Config.GetMode(key);
                    continue;
                }

                // Skip if mode does not require srcCopy
                if (!Program.Config.Modes[_currentMode].SrcCopy)
                {
                    continue;
                }

                // Check for key validity
                if (Program.Config.ContainsKey(_currentMode, key))
                {
                    Thread thr = new Thread(() => RoboCopy.Copy(_currentMode, key));
                    thr.Start();
                    threadBatch.Add(thr);
                    continue;
                }
            }

            // Wait for RoboCopy to complete
            foreach (var thread in threadBatch)
            {
                thread.Join();
            }
        }


















        public static void WriteLineBreak()
        {
            WriteLineBreak('-');
        }
        public static void WriteLineBreak(char c)
        {
            WriteLineBreak(c, 80);
        }
        public static void WriteLineBreak(char c, int len)
        {
            Console.WriteLine(new String(c, len));
        }
        public static void WriteLine(string line)
        {
            WriteLine(line, "-");
        }
        public static void WriteLine(string line, string prefix)
        {
            Console.WriteLine($"[{prefix}] {line}");
        }
    }
}
