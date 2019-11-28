using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace ScriptExDee
{
    /// <summary>
    /// A program to blow your socks off but not blow you off.
    /// Get ready to easily execute a bunch of scripts and shell commands
    /// for the purpose of automating software installation, QC, and maybe
    /// even somebody's job at some point. 
    /// 
    /// Fingers crossed it won't be mine though.
    /// 
    /// Written by Haohan Liu, but don't come askin' if shit breaks.
    /// </summary>
    class Program
    {
        // Program information
        public static string Version = "19.11.28";
        public static string Title = "ScriptExDee";
        public static string Quote = Quotes.GetQuote();

        // Configuration file
        public static string ConfigFile = "AppConfig.xml";
        public static AppConfig Config = null;

        // Config serialization thread
        public static Thread configThread = new Thread(
            () => { Config = XMLHandler.Initialise(ConfigFile); }
            );

        // Initialisation threads
        public static Thread sysInfoWorker = new Thread(SysInfo.GatherSysInfo);
        public static Thread powerWorker = new Thread(PowerControl.SetToPerformance);
        public static Thread wupWorker = new Thread(WUpdateHandler.WUP);
        

        /// <summary>
        /// Main entrypoint of the program.
        /// </summary>
        static void Main(string[] args)
        {
            // Start program initialization threads
            configThread.Start();
            sysInfoWorker.Start();
            powerWorker.Start();
            wupWorker.Start();            

            // Launch terminal
            TerminalTheme.ApplyTheme();
            TitleScreen.ShowTitle();
            Terminal.Start();
        }
    }
}
