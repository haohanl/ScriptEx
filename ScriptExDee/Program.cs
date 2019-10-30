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
        // Configuration file constants
        public static string ConfigFilename = "AppConfig.xml";

        // Program PATHs
        public static string RootPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string DriverLetter = Path.GetPathRoot(Environment.CurrentDirectory);
        public static string SourcePath = Environment.ExpandEnvironmentVariables(DriverLetter + "FILL IN LATER");
        public static string DestinationPath = Environment.ExpandEnvironmentVariables(DriverLetter + "FILL IN LATER");


        /// <summary>
        /// Main entrypoint of the program.
        /// </summary>
        static void Main(string[] args)
        {
            TerminalTheme.ApplyTheme();

            Console.ReadKey();
        }
    }
}
