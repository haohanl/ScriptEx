using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;

namespace ScriptExDee
{
    /// <summary>
    /// Implement Windows RoboCopy functionality
    /// 
    /// Written by Haohan Liu
    /// </summary>
    static class RoboCopy
    {
        // Commandline constants
        private static readonly string Shell = "cmd.exe";
        private static readonly string Command = "ROBOCOPY";
        private static readonly string Params = @"/e /nc /ns /np /NJH /NDL /NFL /MT";



        // Execute Testing RoboCopy
        // Currently unused. Testing folder transferred via Batch File
        public static void CopyTesting()
        {
            Terminal.hRule();
            Terminal.WriteLine("Please wait for the testing folder to be transferred");
            Copy(AppConfig.TestPath, AppConfig.TestDestPath, "Testing folder");
            Terminal.WriteLine("Ready for user commands");
        }

        public static void Copy(AppConfigScript script)
        {
            string descStr = $"'{script.Key}' | {script.Desc}";

            if (script.FullSourcePath() == null)
            {
                return;
            }

            Copy(script.FullSourcePath(), script.FullDestPath(), descStr);
        }

        // Execute robocopy
        public static void Copy(string srcPath, string destPath, string desc)
        {
            var proc = new Process();

            // Setup RoboCopy parameters
            proc.StartInfo.FileName = Shell;
            proc.StartInfo.Arguments = $@"/C {Command} ""{srcPath}"" ""{destPath}"" {Params}";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            //proc.StartInfo.WorkingDirectory = Program.RootPath;

            // Start RoboCopy
            proc.Start();
            Terminal.WriteLine($"Initiated RoboCopy | {desc}");

            // Catch RoboCopy log
            string log = proc.StandardOutput.ReadToEnd().Trim();

            // Extract transfer data
            var transfer = Regex.Match(log, @"(\d+\.\d+ [mgkt])").Groups;

            // Check for errors. If found, output raw log
            if (transfer[0].Value == "")
            {
                Terminal.WriteLine(log, "!");
            }
            // Otherwise, output size info
            else
            {
                Terminal.WriteLine($"Completed Robocopy | {desc} | {transfer[0].Value.ToUpper() + "B"}");
            }

            // Terminate proc
            proc.WaitForExit();
            proc.Close();
        }

        public static void BatchCopy(List<AppConfigScript> scripts)
        {
            List<Thread> threadBatch = new List<Thread>();

            // Initiate all threads
            foreach (var script in scripts)
            {
                Thread thr = new Thread(() => Copy(script));
                thr.Start();
                threadBatch.Add(thr);
            }

            // Block all threads
            foreach (var thread in threadBatch)
            {
                thread.Join();
            }
        }
    }
}
