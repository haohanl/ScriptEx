using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ScriptExDee_II
{
    /// <summary>
    /// Main class for handling user input and console output
    /// </summary>
    public static class Terminal
    {
        // Terminal static variables
        public static string CurrentMode;
        static bool IsCommandClean;
        static List<string> CommandList;

        // Terminal Theme variables
        public const int TerminalWidth = 120;
        public const int TerminalHeight = 50;
        public const int TSIZE = 119;

        public static void Start()
        {
            string _prevMode = CurrentMode;
            
            ShowModeHeader();

            while (true)
            {
                // Check for mode change
                if (_prevMode != CurrentMode)
                {
                    ShowNewMode();

                    // Update Terminal title
                    Title.SetMode();
                }

                // Update current mode
                _prevMode = CurrentMode;

                // Read user input
                ReadUserInput();
                if (string.IsNullOrEmpty(CommandList.First()))
                {
                    continue;
                }

                // Check for invoke special case. If found, divert to Shelleton
                if (Program.Config.IsInvokeKey(CommandList.First()[0].ToString()))
                {
                    CommandList[0] = CommandList[0].Substring(1);
                    Shelleton.Shell.Run(string.Join(" ", CommandList));
                    continue;
                }

                // Validate input
                ValidateInput();

                // Display commands running
                if (CommandList.Count() > 0)
                {
                    Console.Write("[#] Commands: ");
                    foreach (string command in CommandList)
                    {
                        Console.Write(command + " ");
                    }
                    Console.WriteLine();
                }
                // Transfer relevant software from remote source
                TransferSoftware(CommandList, CurrentMode);

                // Run all user commands
                RunUserCommands(CommandList);

                // Insert new line
                WriteLineBreak();
            }
        }

        #region # Input validation

        /// <summary>
        /// Intake user input for validation and conversion into CommandItems
        /// </summary>
        static void ReadUserInput()
        {
            // Preprocessing variables
            string _input;

            // Read user inpout
            Console.Write("> ");
            _input = Console.ReadLine();
            CommandList = new List<string>(_input.Trim().Split(' '));
        }

        static void ValidateInput()
        {
            IsCommandClean = true;
            List<string> _cList = new List<string>(CommandList);
            CommandList = ValidateCommands(_cList, CurrentMode);

            if (!IsCommandClean && !Program.Config.Program.IgnoreInvalidCommands)
            {
                CommandList.Clear();
            }
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
                        Terminal.WriteLine("Macro command cannot call itself.", "!");
                        continue;
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
                if (Program.Config.IsCommandKey(_currentMode, key))
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

        #endregion

        #region # Software transfer

        /// <summary>
        /// Transfer all required software from remote source
        /// </summary>
        static void TransferSoftware(List<string> commands, string currentMode)
        {
            // Check for remote drive availability
            RoboCopy.Reinitialise();
            if (RoboCopy.SrcDrive == null)
            {
                return;
            }

            // Prepare new check
            string _currentMode;
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

                // Check for special keys
                if (Program.Config.IsSpecialKey(key))
                {
                    continue;
                }

                // Skip if mode does not require srcCopy
                if (!Program.Config.Modes[_currentMode].SrcCopy)
                {
                    continue;
                }

                // Check for key validity
                if (Program.Config.IsCommandKey(_currentMode, key))
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

        #endregion

        #region # Command execution

        static void RunUserCommands(List<string> commands)
        {
            // declare threadblock
            List<Thread> _threadList = new List<Thread>();

            // Loop through all commands
            foreach (var key in commands)
            {
                // Check for mode keys
                if (Program.Config.IsModeKey(key))
                {
                    ThreadBlock(_threadList);
                    if (CurrentMode.Equals(Program.Config.GetMode(key)))
                    {
                        WriteLine($"Already in '{Program.Config.GetMode(key)} Mode'", "*");
                        continue;
                    }
                    WriteLine($"Switched to '{Program.Config.GetMode(key)} Mode'", "*");
                    CurrentMode = Program.Config.GetMode(key);
                    continue;
                }

                // Check for special keys
                if (Program.Config.IsThreadBlock(key))
                {
                    WriteLine($"Start thread batch | {_threadList.Count()} threads", "|");
                    ThreadBlock(_threadList);
                    WriteLine("Ended thread batch.", "|");
                    continue;
                }
                if (Program.Config.IsHelpKey(key))
                {
                    ShowHelp(CurrentMode);
                    continue;
                }

                // Check for key validity
                if (Program.Config.IsCommandKey(CurrentMode, key))
                {
                    Thread _cmdThr = new Thread(() => RunCommand(CurrentMode, key));
                    _cmdThr.Start();

                    // check for threadblocking
                    if (Program.Config.Modes[CurrentMode].Commands[key].IgnoreThreadBlock)
                    {
                        continue;
                    }

                    _threadList.Add(_cmdThr);
                    continue;
                }
            }

            // Wait for user commands to complete
            ThreadBlock(_threadList);
        }


        static void RunCommand(string mode, string key)
        {
            // Get relevant config data
            CommandItem _command = Program.Config.GetCommandItem(mode, key);
            ModeConfig _mode = Program.Config.Modes[mode];

            // Get destination path
            string _dstRoot = Environment.ExpandEnvironmentVariables(_mode.DstModeRoot);
            string _dstPath = _command.GetDstPath(_dstRoot);

            // search for matching executable
            string[] _files;
            try
            {
                _files = Directory.GetFiles(_dstPath, _command.Exec);
                Array.Sort(_files);
            }
            catch (Exception)
            {
                WriteLine($"Local files do not exist for '{_command.Name}'.", "!");
                return;
            }

            // check for matching files
            if (_files.Length == 0)
            {
                // terminate if no files found
                WriteLine($"'{_command.Exec}' not found at '{_dstPath}' | '{key}' | {_command.Name}", "!");
                return;
            }
            if (_files.Length > 1)
            {
                // inform that the first file in array is being used
                WriteLine($"Multiple matching files found. Using '{_files[0]}'", "!");
            }

            // set local exec path
            string _path = Path.Combine(_dstPath, _files[0]);

            // delay command execution, if specified
            if (_command.Delay > 0)
            {
                WriteLine($"Starting in {_command.Delay} ms | '{key}' | {_command.Name}");
                Thread.Sleep(_command.Delay);
            }

            // attempt to start process
            Process _proc;
            try
            {
                _proc = Process.Start(_path, _command.Args);
            }
            catch (Exception)
            {
                WriteLine($"File not found at '{_path}' | '{key}' | {_command.Name}", "!");
                return;
            }

            // wait for process completion
            WriteLine($"Initiated {CurrentMode} Command | '{key}' | {_command.Name}");
            try
            {
                _proc.WaitForExit();
                _proc.Close();
            }
            catch (Exception) { }
            WriteLine($"Completed {CurrentMode} Command | '{key}' | {_command.Name}");
        }

        /// <summary>
        /// Join all threads in a list of threads then clear the list
        /// </summary>
        /// <param name="_thrList"></param>
        static void ThreadBlock(List<Thread> _thrList)
        {
            foreach (Thread _thr in _thrList)
            {
                _thr.Join();
            }

            _thrList.Clear();
        }

        #endregion

        #region # Special commands

        static void ShowHelp(string mode, char c='─', char cOffset='┴')
        {
            // Initialise variables
            ModeConfig _mode = Program.Config.Modes[mode];
            List<string> _keys;
            List<string> _cats;
            int offset = (CurrentMode + "MODE COMMANDS").Length + 3;

            // Show mode header
            string _mHeader = $" {CurrentMode.ToUpper()} MODE COMMANDS";
            int _mOffset = _mHeader.Length + 1;
            Console.WriteLine(new String('─', _mOffset) + "┐");
            Console.WriteLine(_mHeader + " │");

            // Collect command keys
            try
            {
                _keys = new List<string>(_mode.Commands.Keys);
            }
            catch (NullReferenceException)
            {
                WriteLine("Mode contains no commands.", "!");
                return;
            }

            // Collect command categories
            try
            {
                _cats = new List<string>(_mode.Categories);
            }
            catch (ArgumentNullException)
            {
                WriteHeaderTitleBreak("COMMANDS", offset, cOffset, c);
                foreach (var _key in _keys)
                {
                    CommandItem _cmd = _mode.Commands[_key];
                    Console.WriteLine(String.Format(" {0, -5} │ {1, -30} │ \\{2, -30} │ \\{3, -15} │ {4}", _key, _cmd.Name, _cmd.SrcPath, _cmd.NetPath, _cmd.Exec));
                }
                return;
            }

            // Iterate through each category
            _keys.Sort();
            bool _hit = true;
            foreach (var _cat in _cats)
            {
                // Write titles
                if (_hit)
                {
                    // Only write header for topmost cat
                    WriteHeaderTitleBreak(_cat.ToUpper(), offset, cOffset, c);
                    _hit = false;
                }
                else
                {
                    WriteTitleBreak(_cat.ToUpper(), c);
                }

                // Find matching keys
                foreach (var _key in _keys)
                {
                    CommandItem _cmd = _mode.Commands[_key];

                    if (_cmd.Category.Equals(_cat))
                    {
                        Console.WriteLine(String.Format(" {0, -5} │ {1, -30} │ \\{2, -30} │ \\{3, -15} │ {4}", _key, _cmd.Name, _cmd.SrcPath, _cmd.NetPath, _cmd.Exec));
                    }
                }
            }
        }

        #endregion

        #region # Terminal formatting

        public static void ShowNewMode()
        {
            Console.Clear();
            ShowModeHeader();
        }

        static void ShowModeHeader()
        {
            // Universal header
            string _tHeader = $" {Program.Title} | Build {Program.Version}";
            int _tOffset = _tHeader.Length + 1;
            Console.WriteLine(new String('═', _tOffset) + "╗");
            Console.WriteLine(_tHeader + " ║");

            WriteHeaderTitleBreak("MODES", _tOffset, '╩', '═');
            ShowCommands(Program.Config.Program.ModeKeys);

            WriteTitleBreak("SPECIAL", '═');
            ShowCommands(Program.Config.Program.SpecialKeys);

            WriteTitleBreak("MACRO", '═');
            ShowCommands(Program.Config.Macros);
            WriteLineBreak('═');

            Console.WriteLine();
            ShowHelp(CurrentMode);
            WriteLineBreak('─');
        }

        static void ShowCommands(Dictionary<string, string> dict)
        {
            foreach (var item in dict)
            {
                Console.WriteLine(String.Format(" {0, -5} │ {1}", item.Key, item.Value));
            }
        }

        static void ShowCommands(Dictionary<string, MacroItem> dict)
        {
            // Initialise variables
            List<string> _keys;

            // Collect command keys
            try
            {
                _keys = new List<string>(dict.Keys);
                _keys.Sort();
            }
            catch (NullReferenceException)
            {
                Console.WriteLine(" No macro commands configured.");
                return;
            }

            // Print macros
            foreach (var _key in _keys)
            {
                MacroItem _macro = dict[_key];
                Console.WriteLine(String.Format(" {0, -5} │ {1, -20} │ {2}", _key, _macro.Name, _macro.SetMode + " " + _macro.Command));
            }
        }

        public static void WriteLineBreak()
        {
            WriteLineBreak('─');
        }
        public static void WriteLineBreak(char c)
        {
            WriteLineBreak(c, TSIZE);
        }
        public static void WriteLineBreak(char c, int len)
        {
            Console.WriteLine(new String(c, len));
        }
        public static void WriteLine()
        {
            Console.WriteLine();
        }
        public static void WriteLine(string line)
        {
            WriteLine(line, "─");
        }
        public static void WriteLine(string line, string prefix)
        {
            Console.WriteLine($"[{prefix}] {line}");
        }

        public static void WriteHeaderTitleBreak(string title, int offset, char cOffset, char c='─', int len=TSIZE)
        {
            int _size = len - title.Length;
            int _ir = (int)Math.Floor((double)_size / 2) - 1;
            int _il = (int)Math.Ceiling((double)_size / 2) - 1;
            string _l = new String(c, _il);
            string _r = new String(c, _ir);

            char[] _cl = _l.ToCharArray();
            _cl[offset] = cOffset;
            string _nl = new String(_cl);

            Console.WriteLine(_nl + " " + title + " " + _r);
        }

        public static void WriteTitleBreak(string title, char c='─', int len=TSIZE)
        {
            int _size = len - title.Length;
            int _ir = (int)Math.Floor((double)_size / 2) - 1;
            int _il = (int)Math.Ceiling((double)_size / 2) - 1;
            string _l = new String(c, _il);
            string _r = new String(c, _ir);
            Console.WriteLine(_l + " " + title + " " + _r);
        }
        #endregion

        /// <summary>
        /// Controls the terminal window title
        /// </summary>
        public static class Title
        {
            // Title states
            public static string WUPText = "";
            public static string ProgramMode = "SYSTEM SUMMARY";
            static readonly string ProgramTitle = $"{Program.Title} <{Program.Version}>";

            // Set the terminal title
            public static void Update()
            {
                if (WUPText != "")
                {
                    Console.Title = $"{ProgramTitle} - {ProgramMode} - {WUPText}";
                }
                else
                {
                    Console.Title = $"{ProgramTitle} - {ProgramMode}";
                }
            }

            public static void SetMode()
            {
                SetMode(Terminal.CurrentMode.ToUpper() + " MODE");
            }
            // Set the terminal mode
            public static void SetMode(string mode)
            {
                ProgramMode = mode;
                Update();
            }

            // Set the WUP status
            public static void SetWUP(string wup)
            {
                WUPText = wup;
                Update();
            }
        }

        /// <summary>
        /// Sets the theme of the terminal program.
        /// and terminal opacity.
        /// </summary>
        public static class Theme
        {
            // Code snippet from "https://stackoverflow.com/questions/3369993/how-to-set-a-console-application-window-to-be-the-top-most-window-c"
            // Used to keep console on top.
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool SetWindowPos(
                IntPtr hWnd,
                IntPtr hWndInsertAfter,
                int x,
                int y,
                int cx,
                int cy,
                int uFlags);

            //private const int HWND_TOPMOST = -1;
            //private const int SWP_NOMOVE = 0x0002;
            //private const int SWP_NOSIZE = 0x0001;

            // Used to set opacity of terminal
            [DllImport("user32.dll")]
            static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
            // Code snippet from https://www.pinvoke.net/default.aspx/Structures.COLORREF
            // For conversion from COLORREF to uint
            private static uint MakeCOLORREF(byte r, byte g, byte b)
            {
                return (((uint)r) | (((uint)g) << 8) | (((uint)b) << 16));
            }

            public static void ApplyTheme()
            {
                IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;

                // Keep window on top
                //SetWindowPos(hWnd,
                //    new IntPtr(HWND_TOPMOST),
                //    0, 0, 0, 0,
                //    SWP_NOMOVE | SWP_NOSIZE);

                // Set opacity
                SetLayeredWindowAttributes(hWnd,
                    MakeCOLORREF(0, 0, 0),
                    210, // Opacity number (0-255)
                    0x00000002
                    );

                // Set console size to be default width, extended height
                Console.SetWindowSize(Terminal.TerminalWidth, Terminal.TerminalHeight);
            }
        }

        /// <summary>
        /// Disable terminal quick edit mode
        /// Code from: https://stackoverflow.com/questions/13656846/how-to-programmatic-disable-c-sharp-console-applications-quick-edit-mode
        /// </summary>
        public static class DisableConsoleQuickEdit
        {

            const uint ENABLE_QUICK_EDIT = 0x0040;

            // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
            const int STD_INPUT_HANDLE = -10;

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll")]
            static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

            [DllImport("kernel32.dll")]
            static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

            internal static bool Go()
            {

                IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

                // get current console mode
                uint consoleMode;
                if (!GetConsoleMode(consoleHandle, out consoleMode))
                {
                    // ERROR: Unable to get console mode.
                    return false;
                }

                // Clear the quick edit bit in the mode flags
                consoleMode &= ~ENABLE_QUICK_EDIT;

                // set the new mode
                if (!SetConsoleMode(consoleHandle, consoleMode))
                {
                    // ERROR: Unable to set console mode
                    return false;
                }

                return true;
            }
        }
    }
}
