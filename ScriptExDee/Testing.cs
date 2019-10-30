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
    class Testing
    {
        static void Start(AppConfigTest test)
        {
            string execPath = Path.Combine(test.FullDestPath(), test.Exec);
            try
            {
                // Set Delay
                Thread.Sleep(test.Delay);
                // Start installation
                var proc = Process.Start(execPath, test.Args);
                Terminal.WriteLine($"Initiated Testing | '{test.Key}' | {test.Desc}");
                proc.WaitForExit();
                proc.Close();
                Terminal.WriteLine($"Completed Testing | '{test.Key}' | {test.Desc}");
            }
            catch (Exception)
            {
                Terminal.WriteLine($" File not found at '{execPath}' | '{test.Key}' | {test.Desc}", "!");
            }
        }

        public static Thread StartTest(AppConfigTest test)
        {
            Thread thr = new Thread(() => Start(test));
            thr.Start();
            return thr;
        }
    }
}
