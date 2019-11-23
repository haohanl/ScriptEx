using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ScriptExDee
{
    /// <summary>
    /// Sets the theme of the terminal program. Mainly 'keep on top'
    /// and terminal opacity.
    /// 
    /// Written by Haohan Liu
    /// </summary>
    static class TerminalTheme
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
        }
    }
}
