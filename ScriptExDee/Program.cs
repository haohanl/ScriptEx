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
        public static string Version = "1031";
        public static string Title = "ScriptExDee";
        public static string Quote = "RGB stands for Real Gnarly BBs";

        // Configuration file
        public static string ConfigFile = "AppConfig.xml";
        public static AppConfig Config = XMLHandler.Initialise(ConfigFile);

        // Program PATHs
        public static string RootPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string DriveLetter = Path.GetPathRoot(Environment.CurrentDirectory);

        //public static string DriveLetter = @"H:\";

        public static string SourcePath = Environment.ExpandEnvironmentVariables(DriveLetter + Config.RoboCopy.SourceRoot);
        public static string DestPath = Environment.ExpandEnvironmentVariables(Config.RoboCopy.DestRoot);
        public static string TestPath = Environment.ExpandEnvironmentVariables(DriveLetter + Config.RoboCopy.TestRoot);
        public static string TestDestPath = Environment.ExpandEnvironmentVariables(Config.RoboCopy.TestDestRoot);


        /// <summary>
        /// Main entrypoint of the program.
        /// </summary>
        static void Main(string[] args)
        {
            TerminalTheme.ApplyTheme();

            Terminal.Start();
        }
    }
}
