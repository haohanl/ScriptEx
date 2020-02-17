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
            ManagementObjectSearcher pSearcher = new ManagementObjectSearcher(@"Root/Microsoft/Windows/Storage", "select * from MSFT_DISK");
            foreach (ManagementObject m in pSearcher.Get())
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


                    // Format 
                    ManagementObjectSearcher vSearcher = new ManagementObjectSearcher("select * from Win32_Volume");
                    foreach (ManagementObject n in vSearcher.Get())
                    {

                        if ((n["DriveType"].ToString() == HARD_DRIVE) && (n["FileSystem"] == null))
                        {
                            // https://docs.microsoft.com/en-au/previous-versions/windows/desktop/stormgmt/format-msft-volume
                            n.InvokeMethod("Format", new object[] { "NTFS", true, 8192, "New Volume", false });
                            Console.WriteLine($"[-] {n["DriveLetter"].ToString()} CREATED");
                        }

                    }
                    vSearcher.Dispose();


                }
                else
                {
                    Console.WriteLine(" | IGNORED");
                }
            }
            pSearcher.Dispose();
            Terminal.WriteLine("ALL DRIVES INITIALISED AND PARTITIONED", "*");

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

        /// <summary>
        /// Delete all folders and files on desktop
        /// TODO: Add yml config blacklist and whitelist support
        /// </summary>
        public static void ClearDesktop()
        {
            string _desktop = @"%USERPROFILE%\Desktop";
            string _path = Environment.ExpandEnvironmentVariables(_desktop);
            var _di = new DirectoryInfo(_path);

            foreach (var item in _di.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                item.Attributes = FileAttributes.Normal;
            }

            var _files = _di.GetFiles();
            var _folders = _di.GetDirectories();
            foreach (var file in _files)
            {
                if (file.Extension != ".lnk")
                {
                    try
                    {
                        File.Delete(file.FullName);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            foreach (var folder in _folders)
            {
                try
                {
                    Directory.Delete(folder.FullName, true);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
}
