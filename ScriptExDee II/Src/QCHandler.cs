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

namespace ScriptExDee_II
{
    static class QCHandler
    {
        /// <summary>
        /// Format and initialise all drives. Code adapted from Magnus Hjorth
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
                m.InvokeMethod("Initialize", new object[] { GPT });   // Will have no effect on intialized disks

                Console.Write("[-] " + m["Model"].ToString().Trim());

                if (m["NumberOfPartitions"].ToString().Equals("0"))
                {
                    // https://docs.microsoft.com/en-au/previous-versions/windows/desktop/stormgmt/createpartition-msft-disk
                    // Use 1MB alignment = 1048576 bytes
                    m.InvokeMethod("CreatePartition", new object[] { null, true, null, 1048576, null, true, null, null, false, false });
                    // Thread.Sleep(1000);
                    Console.WriteLine(" | INITIALISED");
                }
                else
                {
                    Console.WriteLine(" | IGNORED");
                }
            }
            searcher.Dispose();
            Terminal.WriteLine("ALL DRIVES INITIALISED", "*");
            Terminal.WriteLineBreak();

            // Format 
            searcher = new ManagementObjectSearcher("select * from Win32_Volume");
            foreach (ManagementObject m in searcher.Get())
            {

                if ((m["DriveType"].ToString() == HARD_DRIVE) && (m["FileSystem"] == null))
                {
                    // https://docs.microsoft.com/en-au/previous-versions/windows/desktop/stormgmt/format-msft-volume
                    m.InvokeMethod("Format", new object[] { "NTFS", true, 8192, "New Volume", false });
                    Console.WriteLine($"[-] {m["DriveLetter"].ToString()} CREATED");
                }

            }
            searcher.Dispose();
            Terminal.WriteLine("ALL DRIVES FORMATTED", "*");

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
            string sluiPath = Path.Combine(slui, "slui.exe");
            ProcessStartInfo sInfo = new ProcessStartInfo(sluiPath);
            Process p = new Process
            {
                StartInfo = sInfo
            };
            p.Start();
            p.WaitForExit();
            p.Dispose();
        }

        /// <summary>
        /// Delete the Superposition folder
        /// </summary>
        public static void ClearSuperposition()
        {
            string superPath = Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), "Superposition");

            if (Directory.Exists(superPath))
            {
                DirectoryInfo _di = new DirectoryInfo(superPath);

                foreach (FileInfo file in _di.EnumerateFiles())
                {
                    file.Delete();

                }
                foreach (DirectoryInfo _dir in _di.EnumerateDirectories())
                {
                    _dir.Delete(true);
                }
                _di.Delete();
            }
        }

        /// <summary>
        /// Delete the HEAVEN folder
        /// </summary>
        public static void ClearHeaven()
        {
            string superPath = Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), "HEAVEN");

            if (Directory.Exists(superPath))
            {
                DirectoryInfo _di = new DirectoryInfo(superPath);

                foreach (FileInfo file in _di.EnumerateFiles())
                {
                    file.Delete();

                }
                foreach (DirectoryInfo _dir in _di.EnumerateDirectories())
                {
                    _dir.Delete(true);
                }
                _di.Delete();
            }
        }
    }
}
