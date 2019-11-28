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
        
        public static string Title = $@"      _____           _       __  ______     ____          
     / ___/__________(_)___  / /_/ ____/  __/ __ \___  ___ 
     \__ \/ ___/ ___/ / __ \/ __/ __/ | |/_/ / / / _ \/ _ \
    ___/ / /__/ /  / / /_/ / /_/ /____>  </ /_/ /  __/  __/
   /____/\___/_/  /_/ .___/\__/_____/_/|_/_____/\___/\___/ 
                   /_/ {Program.Quote}
   Haohan Liu (c) 2019";

        private const string TAB = "   ";

        public static void ShowTitle()
        {
            // Set Program window title
            Terminal.Title.Update();

            // Animate starting text (for fanciness and to give threads time to work)
            AnimateWrite(Title, 40);
            hRule();

            // Output program status. Sleep for emphasis on program "complexity"
            Console.Write(TAB + $"Loading '{Program.ConfigFile}'...");
            Program.configThread.Join();
            Thread.Sleep(20);
            Console.Write(" done.\n");

            Thread.Sleep(20);
            Console.Write(TAB + "Enable Performance Mode...");
            Thread.Sleep(20);
            Program.powerWorker.Join();
            Console.Write(" done.\n");

            Thread.Sleep(20);
            Console.Write(TAB + "Starting WUpdateHandler...");
            Thread.Sleep(20);
            Console.Write(" done.\n");

            Thread.Sleep(20);
            Console.Write(TAB + "Collecting WMI SysInfo....");
            Program.sysInfoWorker.Join();
            Console.Write(" done.\n");

            

            // Initialise PATHS
            Program.Config.InitialiseSrcDrive();
            Program.Config.InitialisePaths();

            // Show Sys summary
            Write("");
            WriteSysSummary();


            // Wait for user input
            Write();
            Terminal.Title.SetMode("READY");
            Write($"All good? (Press any key to continue. Don't forget to move '{Program.Config.RoboCopy.TestRoot}'!)");
            ReadModeKey();
            // ENABLE FOR 
            //if (Console.ReadKey().Key == ConsoleKey.Enter)
            //{
            //    RoboCopy.CopyTesting();
            //}
            Console.Clear();
        }

        static void Write(string line="", string tab=TAB)
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

        public static void WriteSysSummary()
        {
            Write("=================== SYSTEM SUMMARY =====================");

            Write($"CPU : {SysInfo.CPU.Name}");
            Write($"      {SysInfo.CPU.Description}");

            Write();

            Write($"GPU : {SysInfo.GPU.Name} ({SysInfo.GPU.DriverVersion}) [{SysInfo.GPU.DriverDate}]");

            Write();

            Write($"RAM : {SysInfo.RAM.TotalCapacity.ToString("F")}GB ({SysInfo.RAM.NumSticks}×{SysInfo.RAM.SingleCapacity}GB) [{SysInfo.RAM.Speed}MHz]");
            Write($"      {SysInfo.RAM.Name} ({SysInfo.RAM.Manufacturer})");

            Write();

            Write($"MOBO: {SysInfo.MOBO.Name} ({SysInfo.MOBO.Manufacturer})");
            Write($"S/N : {SysInfo.MOBO.SerialNumber}");
            Write($"BIOS: {SysInfo.MOBO.BIOS} [{SysInfo.MOBO.BIOSDate}]");

            Write();

            Console.Write(TAB + "DISK:");
            foreach (Drive drive in SysInfo.Drives.List)
            {
                if (drive.MediaType != "Removable Media")
                {
                    Console.WriteLine($" {drive.Name} ({drive.Size.ToString("N0")} GB) [{drive.Partitions} Partitions]");
                    Console.Write(TAB + "     ");
                }
            }
        }


        public static void ReadModeKey()
        {
            var _input = Console.ReadKey();

            string _modekey = "!" + _input.KeyChar;

            // check for mode
            if (Terminal.SpecialKeys.All.Contains(_modekey))
            {
                // don't set mode to threadblock
                if (_modekey != Terminal.SpecialKeys.ThreadBlock)
                {
                    Terminal.State.Mode = _modekey;
                }
            }
        }
    }

}
