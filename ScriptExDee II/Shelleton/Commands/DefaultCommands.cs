using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ScriptExDee_II.Shelleton.Commands
{
    public static class DefaultCommands
    {

        #region # Terminal accessible commands
        /// <summary>
        /// Show shelleton help
        /// </summary>
        public static void h()
        {
            Console.WriteLine("SHELLETON COMMANDS (<> Required argument, [] Optional argument):");
            Help();
        }

        /// <summary>
        /// Show system information
        /// </summary>
        public static void si()
        {
            SysSummary();
        }

        /// <summary>
        /// Reset console window text to default
        /// </summary>
        public static void cls()
        {
            Terminal.ShowNewMode();
        }

        /// <summary>
        /// Prepare the system for manual qc
        /// </summary>
        public static void qc()
        {
            cleanup();
            fd();
            act();
            si();
        }

        /// <summary>
        /// Open windows activation prompt for key entry
        /// </summary>
        public static void act()
        {
            QCHandler.WinActivation();
        }

        /// <summary>
        /// Run automatic drive partitioning script
        /// </summary>
        public static void fd()
        {
            QCHandler.FormatDrives();
        }

        /// <summary>
        /// Toggle program self cleanup protocols
        /// </summary>
        /// <param name="toggle"></param>
        public static void cleanup(bool toggle=true)
        {
            QCHandler.ClearHeaven();
            QCHandler.ClearSuperposition();
            CleanupOnExit(toggle);
            Console.WriteLine(" Program will self cleanup after exit.");
        }

        /// <summary>
        /// Toggle program auto start task
        /// </summary>
        /// <param name="toggle"></param>
        public static void autostart(bool toggle=true)
        {
            if (toggle)
            {
                CreateTaskService();
            }
            else
            {
                DeleteTaskService();
            }
        }

        /// <summary>
        /// Check if path exists
        /// </summary>
        /// <param name="path"></param>
        public static void checkpath(string path)
        {
            if (Directory.Exists(path))
            {
                Console.WriteLine($"Path exists [{path}]");
            }
            else
            {
                Console.WriteLine($"Path not found [{path}]");
            }
        }

        /// <summary>
        /// Retrieve CopyTransferInformation from a specific command key
        /// </summary>
        public static void cti(string mode, string key)
        {
            // check validity of mode and key
            if (!Program.Config.Program.ModeKeys.Values.Contains(mode))
            {
                Console.WriteLine("Invalid mode");
                return;
            }
            if (!Program.Config.IsCommandKey(mode, key))
            {
                Console.WriteLine("Invalid command key");
                return;
            }

            // perform cti check
            CommandTransferInfo _cti = new CommandTransferInfo(mode, key);
            Terminal.WriteLineBreak();
            Console.WriteLine("   NAME: " + _cti.Name);
            Console.WriteLine(" NEWEST: " + _cti.NewestSrcPath);
            Console.WriteLine("    SRC: " + _cti.SrcPath);
            Console.WriteLine("    NET: " + _cti.NetPath);
            Console.WriteLine("    DST: " + _cti.DstPath);
            Terminal.WriteLineBreak();
        }

        /// <summary>
        /// Toggle RestartHandler AwaitingRestart switch
        /// </summary>
        /// <param name="toggle"></param>
        public static void ars(bool toggle=true)
        {
            RestartHandler.AwaitingRestartState(toggle);
        }

        /// <summary>
        /// Restart the computer
        /// </summary>
        public static void restart()
        {
            RestartHandler.RestartSystem();
        }
        #endregion


        #region # Program interface methods
        static void Help()
        {
            Terminal.WriteLineBreak();
            foreach (var key in Shell.GetCommands())
            {
                List<string> Arguments = new List<string>(key.RequiredArgs);
                Arguments.AddRange(key.OptionalArgs);
                Console.WriteLine(string.Format(" {0, -35} │ {1}", 
                    key.Name, string.Join(" ", Arguments)
                    ));
            }
            Terminal.WriteLineBreak();
        }
        
        static void SysSummary()
        {
            SysInfo.Initialise();
            Terminal.WriteLineBreak();
            SysInfo.SysSummary(" ");
            Terminal.WriteLineBreak();
        }

        static void CreateTaskService()
        {
            TaskHandler.CreateTaskService();
        }

        static void DeleteTaskService()
        {
            TaskHandler.DeleteTaskService();
        }

        static void CleanupOnExit(bool toggle=true)
        {
            ExitHandler.CleanupOnExit(toggle);
        }
        #endregion
    }
}
