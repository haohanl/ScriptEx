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
        public static string Version = "1103";
        public static string Title = "ScriptExDee";
        public static string Quote = "RGB stands for Real Gnarly BBs";

        // Configuration file
        public static string ConfigFile = "AppConfig.xml";
        public static AppConfig Config = XMLHandler.Initialise(ConfigFile);

        // Program PATHs
        public static string RootPath = AppDomain.CurrentDomain.BaseDirectory;

        public static string DriveLetter = $@"{Config.RoboCopy.SrcDriveLetter}:\";

        public static string SoftPath = Environment.ExpandEnvironmentVariables(DriveLetter + Config.RoboCopy.SoftRoot);
        public static string SoftDestPath = Environment.ExpandEnvironmentVariables(Config.RoboCopy.SoftDestRoot);
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
