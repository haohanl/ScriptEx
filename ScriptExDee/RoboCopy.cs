using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

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


        // Execute robocopy
        public static void Copy(AppConfigScript script)
        {
            var proc = new Process();

            // Setup RoboCopy parameters
            proc.StartInfo.FileName = Shell;
            proc.StartInfo.Arguments = $@"/C {Command} ""{script.FullSourcePath()}"" ""{script.FullDestPath()}"" {Params}";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = Program.RootPath;

            // Start RoboCopy
            proc.Start();
            Terminal.WriteLine($"Initiated RoboCopy | '{script.Key}' | {script.Desc}");

            // Catch RoboCopy log
            string log = proc.StandardOutput.ReadToEnd().Trim();

            // Extract transfer data
            var transfer = System.Text.RegularExpressions.Regex.Match(log, @"(\d+\.\d+ [mgk])").Groups;

            // Check for errors. If found, output raw log
            if (transfer[0].Value == "")
            {
                Terminal.WriteLine(log, "!");
            }
            // Otherwise, output size info
            else
            {
                Terminal.WriteLine($"Completed Robocopy | '{script.Key}' | {script.Desc} |  {transfer[0].Value.ToUpper() + "B"} / {transfer[1].Value.ToUpper() + "B"}");
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
