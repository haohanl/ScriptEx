﻿using System;
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

        public static string Root = System.AppDomain.CurrentDomain.BaseDirectory;

        public static void Run(string exec, string args = "")
        {
            Console.WriteLine($"[{CurrTime()}] {exec} {args} | thread initiated.");
            try
            {
                var proc = System.Diagnostics.Process.Start(exec, args);
                proc.WaitForExit();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"[{CurrTime()}] {exec} {args} | File not found.");
            }
            

            Console.WriteLine($"[{CurrTime()}] {exec} {args} | thread terminated.");
        }

        public static void RunThread(string exec, string args = "")
        {
            ThreadStart stThr = () => Run(exec, args);
            Thread thr = new Thread(stThr);
            thr.Start();
        }

        public static string LocalExec(string exec)
        {
            return Path.Combine(Root, exec);
        }

        public static string CurrTime()
        {
            return DateTime.Now.ToString("hh:mm:ss");
        }
    }
}
