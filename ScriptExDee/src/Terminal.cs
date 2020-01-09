using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace ScriptExDee
{
    /// <summary>
    /// Main class for handling user input and console output
    /// </summary>
    static class Terminal
    {

        /// <summary>
        /// Stores state of the terminal window
        /// </summary>
        public static class State
        {
            /// <summary>
            /// List of unfiltered space separated user command strings
            /// </summary>
            public static List<string> UserInput;

            /// <summary>
            /// List of valid pending user commands
            /// </summary>
            public static List<string> PendingCommands;

            /// <summary>
            /// List of pending file transfers to local dir
            /// </summary>
            public static List<string> PendingSoftwareTransfers;

            /// <summary>
            /// Current operating mode of the terminal
            /// </summary>
            public static string Mode;

            public static void PrintList(List<string> l)
            {
                string[] _tmp = l.ToArray();

                Console.Write("< ");
                foreach (string item in _tmp)
                {
                    Console.Write(item + " ");
                }
                Console.WriteLine(">");
            }
        }

        /// <summary>
        /// Define special command keys for the terminal
        /// </summary>
        public static class SpecialKeys
        {
            public const string QC = "!q";

            public const string Test = "!t";

            public const string Software = "!s";

            public const string ThreadBlock = "|";

            public static readonly string[] All = { QC, Test, Software, ThreadBlock };
        }

        /// <summary>
        /// Controls the terminal window title
        /// </summary>
        public static class Title
        {
            // Title states
            public static string WUPText = "";
            public static string ProgramMode = "INITIALISING";
            static readonly string ProgramTitle = $"{Program.Title} [Build {Program.Version}]";

            // Set the terminal title
            public static void Update()
            {
                Console.Title = $"{ProgramTitle} - {ProgramMode} - {WUPText}";
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
        /// Entrypoint for initiating terminal operations
        /// </summary>
        public static void Start()
        {
            // default to software mode if no mode is active
            if (State.Mode == null)
            {
                State.Mode = SpecialKeys.Software;
            }

            // begin
            StartLoop();
        }

        /// <summary>
        /// Begin terminal input-execution loop
        /// </summary>
        static void StartLoop()
        {
            

            // display initial mode title and header
            ShowCurrentModeTitle();

            // begin loop
            while (true)
            {
                // initialise new state
                State.PendingCommands = new List<string>();
                State.PendingSoftwareTransfers = new List<string>();

                // store starting mode
                string _currMode = State.Mode;

                // process user input
                ReadUserInput();
                ValidateUserInput();

                // transfer prerequisite software
                ExecuteSoftwareTransfers();

                // run all pending commands
                ExecuteUserCommands();

                // if mode has changed, show new mode header text
                if (!(_currMode.Equals(State.Mode)))
                {
                    ShowCurrentModeTitle();
                }

            }
        }


        #region # User input preprocessing

        /// <summary>
        /// Read line of user input and place commands into list of strings
        /// </summary>
        static void ReadUserInput()
        {
            Console.Write("> ");
            string _in = Console.ReadLine();
            State.UserInput = new List<string>(_in.Trim().Split(' '));
        }

        /// <summary>
        /// Validate user input keys and append onto command execution queue
        /// </summary>
        static void ValidateUserInput()
        {
            ValidateUserInput(State.UserInput, State.PendingCommands, State.PendingSoftwareTransfers, State.Mode);
        }
        /// <summary>
        /// Validate user input keys and append onto command execution queue
        /// </summary>
        /// <param name="input">List of user command strings</param>
        /// <param name="mode">Initial mode from which commands are run</param>
        static void ValidateUserInput(List<string> input, string mode)
        {
            ValidateUserInput(input, State.PendingCommands, State.PendingSoftwareTransfers, mode);
        }
        /// <summary>
        /// Validate user input keys and append onto command execution queue
        /// </summary>
        /// <param name="input">List of user command strings</param>
        /// <param name="outCommands">Output for pending commands</param>
        /// <param name="outTransfers">Output for pending transfers</param>
        /// <param name="mode">Initial mode from which commands are run</param>
        static void ValidateUserInput(List<string> input, List<string> outCommands, List<string> outTransfers, string mode)
        {
            // simulate mode changes
            string _startMode = mode;

            // check all user input keys
            foreach (string key in input)
            {
                // check for mode changes
                if (SpecialKeys.All.Contains(key))
                {
                    // add commands to queue
                    outCommands.Add(key);
                    

                    // don't set mode to threadblock
                    if (key == SpecialKeys.ThreadBlock)
                    {
                        continue;
                    }

                    // set new operation mode
                    State.Mode = key;
                    continue;
                }

                // check validity for mode specific commands
                switch (State.Mode)
                {
                    case SpecialKeys.QC:
                        // TODO: Create QC Mode commands
                        break;

                    case SpecialKeys.Test:
                        // expand test stages into individual commands and revalidate
                        if (Program.Config.GetStage(key) != null)
                        {
                            // expanded array
                            string _stageString = Program.Config.GetStage(key).Commands;
                            List<string> _stageKeys = new List<string>(_stageString.Trim().Split(' '));

                            // validate new commands
                            try
                            {
                                ValidateUserInput(_stageKeys, SpecialKeys.Test);
                            }
                            catch (StackOverflowException)
                            {
                                Console.WriteLine("ERR: Test stage error. Stage cannot call self. Program will exit.");
                                Console.ReadKey();
                                Environment.Exit(-1);
                            }
                            
                            break;
                        }
                        // validate single command
                        if (Program.Config.GetTest(key) != null)
                        {
                            outCommands.Add(key);
                            break;
                        }

                        // invalid command handling
                        WriteLine($"'{key}' is not a valid command. Ignoring.", "!");
                        break;

                    case SpecialKeys.Software:
                        // validate single command
                        if (Program.Config.GetScript(key) != null)
                        {
                            outCommands.Add(key);
                            outTransfers.Add(key);
                            break;
                        }

                        // invalid command handling
                        WriteLine($"'{key}' is not a valid command. Ignoring.", "!");
                        break;

                    default:
                        Console.WriteLine("ERR: Invalid operation mode reached. Program will exit.");
                        Console.ReadKey();
                        Environment.Exit(-1);
                        break;
                }
            }

            // reset mode to initial mode
            State.Mode = _startMode;
        }

        #endregion


        #region # User command execution

        /// <summary>
        /// Run ROBOCOPY on all validated user commands
        /// </summary>
        static void ExecuteSoftwareTransfers()
        {
            // create list of script objects
            List<AppConfigScript> _transfers = new List<AppConfigScript>();

            foreach (string key in State.PendingSoftwareTransfers)
            {
                _transfers.Add(Program.Config.GetScript(key));
            }

            // run ROBOCOPY
            RoboCopy.BatchCopy(_transfers);
        }

        /// <summary>
        /// Run all pending user commands according to order and threading
        /// </summary>
        static void ExecuteUserCommands()
        {
            // declare threadblock
            List<Thread> _threadList = new List<Thread>();

            // process all pending commands
            foreach (string key in State.PendingCommands)
            {
                // check for special keys
                if (SpecialKeys.All.Contains(key))
                {
                    // handle threadblock
                    if (key == SpecialKeys.ThreadBlock)
                    {
                        WriteLine("Start thread batch.", "|");
                        ThreadBlock(_threadList);
                        WriteLine("Ended thread batch.", "|");
                        continue;
                    }

                    // set new operation mode
                    if (!(key.Equals(State.Mode)))
                    {
                        State.Mode = key;
                        WriteLine($"Mode changed to '{key}'", "*");
                    }
                    else
                    {
                        WriteLine($"Already in mode '{key}'");
                    }

                    // wait for current mode threads to exit before continuing
                    // NOTE: Change this if simultaneous threads between modes is desired
                    // as manual additions of threadblocks can be added.
                    ThreadBlock(_threadList);

                    

                    continue;
                }

                // run command thread and add thread to threadlist
                switch (State.Mode)
                {
                    case SpecialKeys.QC:
                        // TODO: Create QC Mode commands
                        break;

                    case SpecialKeys.Test:
                        var _test = Program.Config.GetTest(key);
                        Thread _testThr = new Thread(() => RunCommand(_test));
                        _testThr.Start();

                        // check for ignore threadblock flag
                        if (_test.IgnoreThreadBlock)
                        {
                            break;
                        }
                        _threadList.Add(_testThr);
                        break;

                    case SpecialKeys.Software:
                        var _script = Program.Config.GetScript(key);
                        Thread _scriptThr = new Thread(() => RunCommand(_script));
                        _scriptThr.Start();
                        _threadList.Add(_scriptThr);
                        break;

                    default:
                        Console.WriteLine("ERR: Invalid operation mode reached. Program will exit.");
                        Console.ReadKey();
                        Environment.Exit(-1);
                        break;
                }
            }

            // Wait for all commands to finish before accepting new commands
            ThreadBlock(_threadList);
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

        static void RunCommand(AppConfigTest test)
        {
            // path of local executable
            string _path = Path.Combine(test.FullDestPath(), test.Exec);
            Process _proc;

            // delay command execution, if specified
            if (test.Delay > 0)
            {
                Thread.Sleep(test.Delay);
                WriteLine($"Starting in {test.Delay}ms | '{test.Key}' | {test.Desc}");
            }

            // attempt to start process
            try
            {
                _proc = Process.Start(_path, test.Args);
            }
            catch (Exception)
            {
                WriteLine($"File not found at '{_path}' | '{test.Key}' | {test.Desc}", "!");
                return;
            }

            // wait for process completion
            WriteLine($"Initiated Testing | '{test.Key}' | {test.Desc}");
            _proc.WaitForExit();
            _proc.Close();
            WriteLine($"Completed Testing | '{test.Key}' | {test.Desc}");
        }

        static void RunCommand(AppConfigScript script)
        {
            // search for matching executable
            string[] _files = Directory.GetFiles(script.FullDestPath(), script.Exec);
            Array.Sort(_files);

            // check for matching files
            if (_files.Length < 1)
            {
                // terminate if no files found
                WriteLine($"'{script.Exec}' not found at '{script.FullDestPath()}' | '{script.Key}' | {script.Desc}", "!");
                return;
            }
            if (_files.Length > 1)
            {
                // inform that the first file in array is being used
                WriteLine($"Multiple matching files found. Using '{_files[0]}'", "!");
            }

            // set local exec path
            string _path = Path.Combine(script.FullDestPath(), _files[0]);

            // attempt to start process
            Process _proc;
            try
            {
                _proc = Process.Start(_path, script.Args);
            }
            catch (Exception)
            {
                WriteLine($"Cannot start installer '{_path}' | '{script.Key}' | {script.Desc}", "!");
                return;
            }

            // wait for process completion
            WriteLine($"Initiated Install | '{script.Key}' | {script.Desc}");
            _proc.WaitForExit();
            _proc.Close();
            WriteLine($"Completed Install | '{script.Key}' | {script.Desc}");
        }

        #endregion


        #region # Mode formatting & help output

        /// <summary>
        /// Detect current mode and display relevant mode information
        /// </summary>
        static void ShowCurrentModeTitle()
        {
            // Clear console window
            Console.Clear();

            // Detect which title to show
            switch (State.Mode)
            {
                case SpecialKeys.QC:
                    // TODO: Create QC Mode title when commands are added
                    QCModeTitle();
                    break;

                case SpecialKeys.Test:
                    TestModeTitle();
                    break;

                case SpecialKeys.Software:
                    SoftwareModeTitle();
                    break;

                default:
                    Console.WriteLine("ERR: Invalid operation mode reached. Program will exit.");
                    Console.ReadKey();
                    Environment.Exit(-1);
                    break;
            }
        }

        /// <summary>
        /// Print testing mode information
        /// </summary>
        static void TestModeTitle()
        {
            HRule();
            SetTitle("TESTING MODE");
            HRule();

            Console.WriteLine($"DEST: \t'{AppConfig.TestDestPath}'");

            hRule();

            // Print config commands
            foreach (var test in Program.Config.Tests)
            {
                Console.WriteLine($" {test.Key} \t| {test.Desc} --- ['{Path.Combine(Program.Config.RoboCopy.TestRoot, test.DirPath)}']");
            }

            hRule();

            // Print config stages
            foreach (var stage in Program.Config.TestStages)
            {
                Console.WriteLine($" {stage.Key} \t| {stage.Desc} --- ['{stage.Commands}']");
            }
            hRule();
        }


        /// <summary>
        /// Print software mode information
        /// </summary>
        static void SoftwareModeTitle()
        {
            HRule();
            SetTitle("SOFTWARE MODE");
            HRule();

            Console.WriteLine($"SRC: \t'{AppConfig.SoftPath}'");
            //Console.WriteLine($"DEST: \t'{AppConfig.SoftDestPath}'");

            hRule();

            // Print config commands
            int i = 0;
            foreach (var script in Program.Config.Scripts)
            {
                Console.WriteLine($" {script.Key} \t| {script.Desc} --- ['{Path.Combine(Program.Config.RoboCopy.SoftRoot, script.SourcePath)}']");

                i++;
                if (i == 6)
                {
                    hRule();
                }
            }
            hRule();
        }

        /// <summary>
        /// Print QC mode information
        /// </summary>
        static void QCModeTitle()
        {
            HRule();
            SetTitle("QC MODE");
            HRule();

            // Open windows activation dialog
            Terminal.WriteLine("Run Windows Activation Dialog", "1");
            Thread _winActThr = new Thread(QCMode.WinActivation);
            _winActThr.Start();

            // Ensure all drives are partitioned
            Terminal.WriteLine("Partitioning Disk Drives", "2");
            Thread _drvThr = new Thread(QCMode.FormatDrives);
            _drvThr.Start();

            // Cleanup testing folders for heaven and superposition
            QCMode.ClearHeaven();
            QCMode.ClearSuperposition();
            Terminal.WriteLine("Superposition + Heaven folders cleared", "3");

            hRule();

            _drvThr.Join();
            _winActThr.Join();

            HRule();
            SysInfo.GatherSysInfo();
            Console.WriteLine();
            TitleScreen.WriteSysSummary();
            Console.WriteLine();
            HRule();
        }

        #endregion


        #region # Terminal formatting

        // Terminal formatting for lines
        public static void WriteLine(string outStr, string outChar = "-")
        {
            Console.WriteLine($"[{outChar}] {outStr}");
        }


        // Set window title
        public static void SetTitle(string mode)
        {
            Title.SetMode(mode);
            Console.WriteLine($"{Program.Title} [Build {Program.Version}] - {mode}");
        }


        // Horizontal ruling
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

        #endregion
    }
}
