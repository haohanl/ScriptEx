using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace ScriptExDee
{
    /// <summary>
    /// Report system hardware information
    /// </summary>
    static class SysInfo
    {
        static public CPUInfo CPU = null;
        static public GPUInfo GPU = null;
        static public MOBOInfo MOBO = null;
        static public RAMInfo RAM = null;
        static public DriveInfo Drives = null;
        static public LogicalDiskInfo LogicalDisks = null;
        static public bool Initialised = false;

        public static void GatherSysInfo()
        {
            // Attempt to gather info

            try
            {
                CPU = new CPUInfo();
            }
            catch (Exception)
            {
                CPU = null;
                return;
            }

            try
            {
                GPU = new GPUInfo();
            }
            catch (Exception)
            {
                GPU = null;
                return;
            }

            try
            {
                MOBO = new MOBOInfo();
            }
            catch (Exception)
            {
                MOBO = null;
                return;
            }

            try
            {
                RAM = new RAMInfo();
            }
            catch (Exception)
            {
                RAM = null;
                return;
            }

            try
            {
                Drives = new DriveInfo();
            }
            catch (Exception)
            {
                Drives = null;
                return;
            }

            try
            {
                LogicalDisks = new LogicalDiskInfo();
            }
            catch (Exception)
            {
                LogicalDisks = null;
                return;
            }


            // Set to initialised
            Initialised = true;
        }
    }

    /// <summary>
    /// Tools for navigating WMI objects to get system information
    /// </summary>
    class SysChecker
    {
        // Directory of Win32 object
        public ManagementObjectCollection Collection;

        /// <summary>
        /// Initialise Win32 object for searching
        /// </summary>
        /// <param name="wmiObject">Win32 object name</param>
        public SysChecker(string wmiObject)
        {
            using (var searcher = new ManagementObjectSearcher($@"Select * From {wmiObject}"))
            {
                Collection = searcher.Get();
            }
        }

        /// <summary>
        /// Get full list of selected attributes in the Win32 object
        /// </summary>
        /// <param name="attr">Attribute name in Win32 object</param>
        /// <returns></returns>
        public List<string> Get(string attr)
        {
            List<string> Vals = new List<string>();

            foreach (ManagementObject obj in Collection)
            {
                Vals.Add(obj[attr].ToString().Trim());
            }

            return Vals;
        }

        /// <summary>
        /// Only return the first occurance of attribute
        /// </summary>
        /// <param name="attr">Attribute name in Win32 object</param>
        /// <returns></returns>
        public string GetFirst(string attr)
        {
            try
            {
                foreach (ManagementObject obj in Collection)
                {
                    return obj[attr].ToString().Trim();
                }
            }
            catch (Exception)
            {
                return null;
            }
            
            return null;
        }

        /// <summary>
        /// Return first occurance of attribute, formatted as a date
        /// </summary>
        /// <param name="attr">Attribute name in Win32 object</param>
        /// <returns></returns>
        public string GetDate(string attr)
        {
            string tmp = GetFirst(attr);

            tmp = tmp.Substring(0, 8);
            tmp = tmp.Insert(4, "/");
            tmp = tmp.Insert(7, "/");

            return tmp;
        }


        //https://ourcodeworld.com/articles/read/294/how-to-retrieve-basic-and-advanced-hardware-and-software-information-gpu-hard-drive-processor-os-printers-in-winforms-with-c-sharp
        static readonly string[] SizeSuffixes = { "bytes", "K", "M", "G", "T", "P", "E", "Z", "Y" };

        public static string SizeSuffix(Int64 value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n2}{1}", adjustedSize, SizeSuffixes[mag]);
        }
        // END SNIPPET (UNUSED)

    }

    /// <summary>
    /// Stores CPU information
    /// </summary>
    class CPUInfo
    {
        public string Name;
        public string Manufacturer;
        public string Description;

        public CPUInfo()
        {
            string wmiObject = "Win32_Processor";
            var searcher = new SysChecker(wmiObject);

            Name = searcher.GetFirst("Name");
            Manufacturer = searcher.GetFirst("Manufacturer");
            Description = searcher.GetFirst("Description");

        }
    }

    /// <summary>
    /// Stores GPU information
    /// </summary>
    class GPUInfo
    {
        public string Name;
        public string DriverVersion;
        public string DriverDate;

        public GPUInfo()
        {
            string wmiObject = "Win32_VideoController";
            var searcher = new SysChecker(wmiObject);

            Name = searcher.GetFirst("Name");
            DriverVersion = searcher.GetFirst("DriverVersion");
            DriverDate = searcher.GetDate("DriverDate");
        }
    }

    /// <summary>
    /// Stores Motherboard information
    /// </summary>
    class MOBOInfo
    {
        public string Name;
        public string Manufacturer;
        public string BIOS;
        public string BIOSDate;
        public string SerialNumber;

        public MOBOInfo()
        {
            string wmiObject = "Win32_BaseBoard";
            var searcher = new SysChecker(wmiObject);

            Name = searcher.GetFirst("Product");
            Manufacturer = searcher.GetFirst("Manufacturer");
            SerialNumber = searcher.GetFirst("SerialNumber");

            wmiObject = "Win32_BIOS";
            searcher = new SysChecker(wmiObject);

            BIOS = searcher.GetFirst("Name");
            BIOSDate = searcher.GetDate("ReleaseDate");
        }
    }

    /// <summary>
    /// Stores RAM information
    /// </summary>
    class RAMInfo
    {
        public long TotalCapacity;
        public long SingleCapacity;
        public int NumSticks;
        public int Speed;
        public string Name;
        public string Manufacturer;

        public RAMInfo()
        {
            string wmiObject = "Win32_PhysicalMemory";
            var searcher = new SysChecker(wmiObject);

            Name = searcher.GetFirst("PartNumber").Trim();
            Manufacturer = searcher.GetFirst("Manufacturer").Trim();
            Speed = Convert.ToInt32(searcher.GetFirst("Speed"));

            TotalCapacity = 0;
            NumSticks = 0;
            foreach (string size in searcher.Get("Capacity"))
            {
                TotalCapacity += Convert.ToInt64(size);
                NumSticks++;
            }
            TotalCapacity = TotalCapacity / (1024 * 1024 * 1024);
            SingleCapacity = TotalCapacity / NumSticks;
        }
    }

    /// <summary>
    /// Stores physical disk information
    /// </summary>
    class DriveInfo
    {
        public List<Drive> List = new List<Drive>();

        public DriveInfo()
        {
            string wmiObject = "Win32_DiskDrive";
            var searcher = new SysChecker(wmiObject);

            foreach (ManagementObject drive in searcher.Collection)
            {
                List.Add(new Drive(drive));
            }

            List = List.OrderBy(o => o.Index).ToList();
        }
    }


    class Drive
    {
        public int Index;
        public string Name;
        public long Size;
        public int Partitions;
        public string MediaType;
        public Drive(ManagementObject drive)
        {
            Index = Convert.ToInt32(drive["index"]);
            Name = drive["Model"].ToString();
            Size = Convert.ToInt64(drive["Size"]) / (1024 * 1024 * 1024);
            Partitions = Convert.ToInt32(drive["Partitions"]);
            MediaType = drive["MediaType"].ToString();

        }
    }



    /// <summary>
    /// Store Logical Disk information, e.g partitions
    /// </summary>
    class LogicalDiskInfo
    {
        public List<LogicalDisk> List = new List<LogicalDisk>();
        public LogicalDiskInfo()
        {
            string wmiObject = "Win32_LogicalDisk";
            var searcher = new SysChecker(wmiObject);

            foreach (ManagementObject drive in searcher.Collection)
            {
                try
                {
                    List.Add(new LogicalDisk(drive));
                }
                catch (Exception)
                {
                    Console.Write("*");
                    continue;
                }
                
            }

            List = List.OrderBy(o => o.DriveLetter).ToList();
        }
    }

    class LogicalDisk
    {
        public string DriveLetter;
        public string Name;
        public LogicalDisk(ManagementObject drive)
        {
            DriveLetter = drive["DeviceID"].ToString();
            Name = drive["VolumeName"].ToString();
        }
    }


}
