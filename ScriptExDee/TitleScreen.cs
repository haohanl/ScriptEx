using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ScriptExDee
{
    static class TitleScreen
    {
        
        public static string Title = $@"         _____           _       __  ______     ____          
        / ___/__________(_)___  / /_/ ____/  __/ __ \___  ___ 
        \__ \/ ___/ ___/ / __ \/ __/ __/ | |/_/ / / / _ \/ _ \
       ___/ / /__/ /  / / /_/ / /_/ /____>  </ /_/ /  __/  __/
      /____/\___/_/  /_/ .___/\__/_____/_/|_/_____/\___/\___/ 
                      /_/ {Quotes.GetQuote()}
      Haohan Liu (c) 2019";

        private static string TAB = "      ";

        public static void ShowTitle()
        {
            // Set Program window title
            Console.Title = $"{Program.Title} [Build {Program.Version}]";

            // Start Initialisation threads
            Thread sysInfoWorker = new Thread(SysInfo.GatherSysInfo);
            sysInfoWorker.Start();

            Thread powerWorker = new Thread(PowerControl.SetToPerformance);
            powerWorker.Start();

            // Animate starting text (for fanciness and to give threads time to work)
            AnimateWrite(Title, 70);
            hRule();

            // Output program status. Sleep for emphasis on program "complexity"
            Console.Write(TAB + $"Loading '{Program.ConfigFile}'...");
            Program.configThread.Join();
            Thread.Sleep(100);
            Console.Write(" done.\n");

            Thread.Sleep(100);
            Console.Write(TAB + "Enable Performance Mode...");
            Thread.Sleep(100);
            powerWorker.Join();
            Console.Write(" done.\n");

            Thread.Sleep(100);
            Console.Write(TAB + "Collecting SysInfo...");
            sysInfoWorker.Join();
            Console.Write(" done.\n");

            // Initialise PATHS
            Program.Config.InitialiseSrcDrive();
            Program.Config.InitialisePaths();

            // Show Sys summary
            Write("");
            Write("=================== SYSTEM SUMMARY =====================");
                                   
            Write($"CPU : {SysInfo.CPU.Name}");
            Write($"GPU : {SysInfo.GPU.Name} ({SysInfo.GPU.DriverVersion})");
           
            Write();

            Write($"RAM : {SysInfo.RAM.TotalCapacity.ToString("F")}GB ({SysInfo.RAM.NumSticks}×{SysInfo.RAM.SingleCapacity}GB) [{SysInfo.RAM.Speed}MHz]");
            Write($"      {SysInfo.RAM.Name} ({SysInfo.RAM.Manufacturer})");
           
            Write();

            Write($"MOBO: {SysInfo.MOBO.Manufacturer} {SysInfo.MOBO.Name}");
            Write($"BIOS: {SysInfo.MOBO.BIOS} ({SysInfo.MOBO.BIOSDate})");

            Write();
           
            Console.Write(TAB + "DISK:");
            foreach (Drive drive in SysInfo.Drives.List)
            {
                if (drive.MediaType != "Removable Media")
                {
                    Console.WriteLine($" {drive.Name} ({drive.Size}) [{drive.Partitions} Partitions]");
                    Console.Write(TAB + "     ");
                }
            }


            // Wait for user input
            Write();
            Write($"All good? (Press any key to continue. Don't forget to move '{Program.Config.RoboCopy.TestRoot}'!)");
            Console.ReadKey();
            // ENABLE FOR 
            //if (Console.ReadKey().Key == ConsoleKey.Enter)
            //{
            //    RoboCopy.CopyTesting();
            //}
            Console.Clear();
        }

        static void Write(string line="", string tab="      ")
        {
            Console.Write($"{tab}{line}\n");
        }

        static void hRule()
        {
            TitleScreen.Write("--------------------------------------------------------");
        }
        static void HRule()
        {
            TitleScreen.Write("========================================================");
        }

        static void AnimateWrite(string title, int delay)
        {
            string[] tmp = title.Split('\n');
            foreach (string line in tmp)
            {
                Console.WriteLine(line);
                Thread.Sleep(delay);
            }
        }
    }

}
