using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Runtime.InteropServices;

namespace ScriptExDee
{
    class QCMode
    {
        /// <summary>
        /// Code from Magnus Hjorth
        /// </summary>
        public static void FormatDrives()
        {
            string HARD_DRIVE = "3";
            int GPT = 2;

            // Create partitions
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"Root/Microsoft/Windows/Storage", "select * from MSFT_DISK");
            foreach (ManagementObject m in searcher.Get())
            {
                // Initialize disk first
                var res = m.InvokeMethod("Initialize", new object[] { GPT });   // Will have no effect on intialized disks

                Console.Write("[*] " + m["Model"].ToString().Trim());

                if (m["NumberOfPartitions"].ToString().Equals("0"))
                {
                    // https://docs.microsoft.com/en-au/previous-versions/windows/desktop/stormgmt/createpartition-msft-disk
                    // Use 1MB alignment = 1048576 bytes
                    var parRes = m.InvokeMethod("CreatePartition", new object[] { null, true, null, 1048576, null, true, null, null, false, false });
                    // Thread.Sleep(1000);
                    Console.WriteLine(" | INITIALISED");
                }
                else
                {
                    Console.WriteLine(" | IGNORED");
                }
            }
            Terminal.WriteLine("ALL DRIVES INITIALISED");
            Terminal.hRule();
            //Thread.Sleep(2000);  // Formatting is unstable if drive letters are not given time to be created

            // Format 
            searcher = new ManagementObjectSearcher("select * from Win32_Volume");
            foreach (ManagementObject m in searcher.Get())
            {
                
                if ((m["DriveType"].ToString() == HARD_DRIVE) && (m["FileSystem"] == null))
                {
                    // https://docs.microsoft.com/en-au/previous-versions/windows/desktop/stormgmt/format-msft-volume
                    var res = m.InvokeMethod("Format", new object[] { "NTFS", true, 8192, "New Volume", false });
                    Console.WriteLine($"[#] {m["DriveLetter"].ToString()} CREATED");
                }

            }

            Terminal.WriteLine("ALL DRIVES FORMATTED");
            Terminal.hRule();

            // In case any drives are offline...
            /*
            searcher = new ManagementObjectSearcher(@"Root/Microsoft/Windows/Storage", "select * from MSFT_Partition");
            foreach (ManagementObject m in searcher.Get())
            {
                if (m["IsOffline"].ToString().ToLower().Equals("true"))
                {
                    var res = m.InvokeMethod("Online", new object[] { });
                }
            }
            */
        }

        /// <summary>
        /// Run Windows Activation
        /// </summary>
        public static void WinActivation()
        {
            // If program is not x64, system32 path gets redirected and this fails to launch
            string slui = Environment.GetFolderPath(Environment.SpecialFolder.System);
            ProcessStartInfo sInfo = new ProcessStartInfo(Path.Combine(slui, "slui.exe"));
            Process p = new Process();
            p.StartInfo = sInfo;
            p.Start();
        }

        public static void ClearSuperposition()
        {
            string superPath = Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), "Superposition");

            if (Directory.Exists(superPath))
            {
                DirectoryInfo di = new DirectoryInfo(superPath);

                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();

                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete(true);
                }
                di.Delete();
            }
        }

        public static void ClearHeaven()
        {
            string superPath = Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), "HEAVEN");

            if (Directory.Exists(superPath))
            {
                DirectoryInfo di = new DirectoryInfo(superPath);

                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();

                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete(true);
                }
                di.Delete();
            }
        }

        public static void LaunchManualChecks()
        {
            //const int SWP_SHOWWINDOW = 0x0040;

            // Launch the four main windows
            ProcessStartInfo devmInfo = new ProcessStartInfo("devmgmt.msc");
            Process devm = new Process();
            devm.StartInfo = devmInfo;
            devm.Start();

            ProcessStartInfo diskmInfo = new ProcessStartInfo("diskmgmt.msc");
            Process diskm = new Process();
            diskm.StartInfo = diskmInfo;
            diskm.Start();

            Thread.Sleep(1000);   // Need to wait for mainwindowtitles to correctly 'guess' the gui window
                                  // Unfortunately, WaitForInputIdle is not sufficient!

            // Reposition mmc class processes

            //Process[] mmcProcs = Process.GetProcessesByName("mmc");
            //foreach (var proc in mmcProcs)
            //{
            //    if (proc.MainWindowTitle.Equals("Disk Management"))
            //    {
            //        var handle = proc.MainWindowHandle;
            //        SetWindowPos(handle, 0, 0, 540, 960, 540, SWP_SHOWWINDOW);
            //    }
            //    if (proc.MainWindowTitle.Equals("Device Manager"))
            //    {
            //        var handle = proc.MainWindowHandle;
            //        SetWindowPos(handle, 0, 960, 540, 960, 540, SWP_SHOWWINDOW);
            //    }
            //}

            // Launch windows update
            string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
            ProcessStartInfo wupInfo = new ProcessStartInfo(Path.Combine(systemFolder, "control.exe"), "/name Microsoft.WindowsUpdate");
            Process wup = new Process();
            wup.StartInfo = wupInfo;
            wup.Start();
            wup.WaitForInputIdle();

            //// System info, explorer is handled by standalone script
            //string sysInfoScript = Path.Combine(Paths.Desktop(), Paths.TEST, Paths.FILES, Paths.QCWINDOWS_SCRIPT);
            //if (File.Exists(sysInfoScript))
            //{
            //    ProcessStartInfo sysInfo = new ProcessStartInfo(sysInfoScript);
            //    Process sys = new Process();
            //    sys.StartInfo = sysInfo;
            //    sys.Start();
            //    sys.WaitForExit();
            //}
        }

    }
}
