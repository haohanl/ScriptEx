using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ScriptEx
{
    class Program
    {
        // App Config
        public static string ConfigFile = "AppConfig.xml";
        public static XMLHandler AppConfig = new XMLHandler(ConfigFile);

        // ScriptEx Paths
        public static string Root = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string DriveLetter = Path.GetPathRoot(Environment.CurrentDirectory);
        //public static string DriveLetter = @"E:\";
        public static string InstallDir = Environment.ExpandEnvironmentVariables(DriveLetter + AppConfig.GetNode("SOURCEROOT"));
        public static string CopyDestDir = Environment.ExpandEnvironmentVariables(AppConfig.GetNode("DESTROOT"));


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

        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;


        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        // Code from https://www.pinvoke.net/default.aspx/Structures.COLORREF
        // For conversion from COLORREF to uint
        private static uint MakeCOLORREF(byte r, byte g, byte b)
        {
            return (((uint)r) | (((uint)g) << 8) | (((uint)b) << 16));
        }

        /// <summary>
        /// Entry point for ScriptEx
        /// </summary>
        /// <param name="args">
        /// To be implemented.
        /// </param>
        static void Main(string[] args)
        {
            // CLI mode
            if (args.Length == 0)
            {
                // Keep window on top
                IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;

                SetWindowPos(hWnd,
                    new IntPtr(HWND_TOPMOST),
                    0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE);

                // Set opacity
                SetLayeredWindowAttributes(hWnd,
                    MakeCOLORREF(0, 0, 0),
                    210, 0x00000002
                    );

                // Start terminal.
                Terminal.Start();
            }

            // Param mode
            else
            {
                Console.WriteLine("To be implemented.");
            }
        }

        // Batch block threads
        public static void ThreadBatchBlock(List<Thread> threadBatch)
        {
            foreach (Thread thread in threadBatch)
            {
                thread.Join();
            }
        }
    }
}
