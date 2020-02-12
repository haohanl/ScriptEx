using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

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
        /// Show DriveInfo data
        /// </summary>
        public static void di()
        {
            ShowDriveInfo();
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
            prun("slui");
        }

        /// <summary>
        /// Run automatic drive partitioning script
        /// </summary>
        public static void fd()
        {
            QCHandler.FormatDrives();
        }

        /// <summary>
        /// Show windows version and key
        /// </summary>
        public static void winkey()
        {
            OSData _os = new OSData();
            _os.GetProdKey();
            Console.WriteLine("{0} | {1}", _os.Name, _os.ProdKey);
        }

        /// <summary>
        /// Restart the computer
        /// </summary>
        public static void restart()
        {
            RestartHandler.RestartSystem();
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
            Console.WriteLine("  VALID: " + _cti.ValidRoboCopy);
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
        public static void ars(bool toggle = true)
        {
            RestartHandler.AwaitingRestartState(toggle);
        }

        /// <summary>
        /// Manually run a process
        /// </summary>
        /// <param name="exe"></param>
        public static void prun(string exe)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(exe);
            Process p = new Process
            {
                StartInfo = sInfo
            };
            p.Start();
            p.WaitForExit();
            p.Dispose();
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
        #endregion


        #region # Program interface methods
        static void ShowDriveInfo()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            Console.WriteLine(" DriveInfo Summary:");
            foreach (DriveInfo d in allDrives)
            {
                Terminal.WriteLineBreak();
                Console.WriteLine(" Drive                 │ {0}", d.Name);
                Console.WriteLine(" Drive type            │ {0}", d.DriveType);
                if (d.IsReady == true)
                {
                    Console.WriteLine(" Volume label          │ {0}", d.VolumeLabel);
                    Console.WriteLine(" File system           │ {0}", d.DriveFormat);
                    Console.WriteLine(
                        " Total available space │ {0:0.00} GiB",
                        (double) d.TotalFreeSpace / 1024 / 1024 / 1024);

                    Console.WriteLine(
                        " Total size of drive   │ {0:0.00} GiB ",
                        (double) d.TotalSize / 1024 / 1024 / 1024);
                }
            }
            Terminal.WriteLineBreak();
        }

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
