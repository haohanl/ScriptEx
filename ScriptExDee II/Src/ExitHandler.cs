using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace ScriptExDee_II
{
    // Code adapted from https://stackoverflow.com/questions/1119841/net-console-application-exit-event
    /// <summary>
    /// Detects and handles exit events for cleanup
    /// </summary>
    static class ExitHandler
    {
        static bool SystemExited = false;

        #region # Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");

            //do your cleanup here
            SelfDestruct();

            Console.WriteLine("Cleanup complete");

            //allow main to run off
            SystemExited = true;

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
        #endregion

        #region ExitHandler interface
        public static void Start()
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);
        }

        #endregion

        #region # Self-destruction protocol
        // Code adapted from https://stackoverflow.com/questions/19689054/is-it-possible-for-a-c-sharp-built-exe-to-self-delete/19689415 and Magnus Hjorth (Builder Companion)
        /// <summary>
        /// Force delete file
        /// </summary>
        private static void ForceDelete(string path)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = "/C choice /C Y /N /D Y /T 2 & Del " + "\"" + path + "\"";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            Process.Start(info);
        }

        private static void SelfDestruct()
        {
            // Delete base executable and config file
            ForceDelete(Path.Combine(Program.ExecPath, Program.Title + ".exe"));
            ForceDelete(Path.Combine(Program.ExecPath, Program.ConfigFile));
        }
        #endregion
    }
}
