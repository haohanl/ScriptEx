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

        // install with executable
        public static void Install(Command cmd)
        {
            try
            {
                Terminal.WriteLine(">", $"'{cmd.AttrVal}' | {cmd.Exec} | install initiated.");
                var proc = System.Diagnostics.Process.Start(cmd.ExecPath, cmd.Args);
                proc.WaitForExit();
                Terminal.WriteLine("<", $"'{cmd.AttrVal}' | {cmd.Exec} | install completed.");
                proc.Close();
                Terminal.WriteLine("x", $"'{cmd.AttrVal}' | {cmd.Exec} | thread terminated.");
            }
            catch (Exception)
            {
                Terminal.WriteLine("!", $"'{cmd.AttrVal}' | {cmd.Exec} | file not found at '{cmd.ExecPath}'");
            }
        }

        // execute a new thread for executing
        public static Thread RunThread(Command cmd)
        {
            ThreadStart stThr = () => Install(cmd);
            Thread thr = new Thread(stThr);
            thr.Start();

            return thr;
        }

        public static string LocalExec(string exec)
        {
            return Path.Combine(Program.Root, exec);
        }
    }
}
