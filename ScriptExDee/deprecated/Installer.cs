using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace ScriptExDee
{
    /// <summary>
    /// Execute installation commands and auto files
    /// 
    /// Written by Haohan Liu
    /// </summary>
    static class Installer
    {
        static void Install(AppConfigScript script)
        {
            string execPath = Path.Combine(script.FullDestPath(), script.Exec);
            try
            {
                // Start installation
                var proc = Process.Start(execPath, script.Args);
                TerminalOld.WriteLine($"Initiated Install | '{script.Key}' | {script.Desc}");
                proc.WaitForExit();
                proc.Close();
                TerminalOld.WriteLine($"Completed Install | '{script.Key}' | {script.Desc}");
            }
            catch (Exception)
            {
                TerminalOld.WriteLine($"File not found at '{execPath}' | '{script.Key}' | {script.Desc}", "!");
            }
        }

        public static Thread StartInstall(AppConfigScript script)
        {
            Thread thr = new Thread(() => Install(script));
            thr.Start();
            return thr;
        }
    }
}
