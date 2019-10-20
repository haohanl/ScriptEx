using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ScriptEx
{
    public static class Installer
    {

        // path to the scriptex exe
        public static string Root = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string DriveLetter = Path.GetPathRoot(Environment.CurrentDirectory);


        // execute an executable
        public static void Run(string path, string args = "")
        {
            try
            {
                Terminal.WriteLine(">", $"{path} {args} | thread initiated.");
                var proc = System.Diagnostics.Process.Start(path, args);
                proc.WaitForExit();
                Terminal.WriteLine(">", $"{path} {args} | thread terminated.");
            }
            catch (Exception)
            {
                Terminal.WriteLine(">", $"{path} {args} | file not found.");
                Terminal.WriteLine(">", $"{path} {args} | thread terminated.");
            }
            

        }

        // execute a new thread for executing
        public static Thread RunThread(string path, string args = "")
        {
            ThreadStart stThr = () => Run(path, args);
            Thread thr = new Thread(stThr);
            thr.Start();

            return thr;
        }

        public static string LocalExec(string exec)
        {
            return Path.Combine(Root, exec);
        }
    }
}
