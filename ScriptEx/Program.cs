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

                // Start terminal.
                Terminal.Start();
            }

            // Param mode
            else
            {
                Console.WriteLine("To be implemented.");
            }
        }
    }
}
