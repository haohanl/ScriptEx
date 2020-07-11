using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;

namespace ScriptExDee_II
{
    /// <summary>
    /// Report system hardware information
    /// </summary>
    static class SysInfo
    {
        static public CPUData CPU = null;
        static public GPUData GPU = null;
        static public MOBOData MOBO = null;
        static public RAMData RAM = null;
        static public DriveData Drives = null;
        static public OSData OS = null;
        static public bool Initialised = false;

        public static void Initialise()
        {
            // Attempt to gather info
            try
            {
                CPU = new CPUData();
            }
            catch (Exception)
            {
                CPU = null;
                return;
            }

            try
            {
                GPU = new GPUData();
            }
            catch (Exception)
            {
                GPU = null;
                return;
            }

            try
            {
                MOBO = new MOBOData();
            }
            catch (Exception)
            {
                MOBO = null;
                return;
            }

            try
            {
                RAM = new RAMData();
            }
            catch (Exception)
            {
                RAM = null;
                return;
            }

            try
            {
                Drives = new DriveData();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Drives = null;
                return;
            }

            try
            {
                OS = new OSData();
                OS.GetProdKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                OS = new OSData();
                return;
            }

            // Set to initialised
            Initialised = true;
        }

        public static void SysSummary(string tab="")
        {
            // constant
            string _div = tab + "     │";

            // Check for SysInfo Initialisation
            if (!SysInfo.Initialised)
            {
                Console.WriteLine(tab + "System Summary requires System Check to initialise.");
                return;
            }

            // Print System Summary
            if (SysInfo.CPU != null)
            {
                Console.WriteLine(tab + $" CPU │ {SysInfo.CPU.Name}");
                Console.WriteLine(_div);
            }
            else
            {
                Console.WriteLine(tab + " CPU │ UNKNOWN");
                Console.WriteLine(_div);
            }

            if (SysInfo.GPU != null)
            {
                Console.WriteLine(tab + $" GPU │ {SysInfo.GPU.Name} ({SysInfo.GPU.DriverVersion}) [{SysInfo.GPU.DriverDate}]");
                Console.WriteLine(_div);
            }
            else
            {
                Console.WriteLine(tab + " GPU │ UNKNOWN");
                Console.WriteLine(_div);
            }


            if (SysInfo.RAM != null)
            {
                Console.WriteLine(tab + $" RAM │ {SysInfo.RAM.TotalCapacity:F}GB ({SysInfo.RAM.NumSticks}×{SysInfo.RAM.SingleCapacity}GB) [{SysInfo.RAM.Speed}MHz]");
                Console.WriteLine(tab + $"     │ {SysInfo.RAM.Name} ({SysInfo.RAM.Manufacturer})");
                Console.WriteLine(_div);
            }
            else
            {
                Console.WriteLine(tab + " RAM │ UNKNOWN");
                Console.WriteLine(_div);
            }

            if (SysInfo.MOBO != null)
            {
                Console.WriteLine(tab + $"MOBO │ {SysInfo.MOBO.Name} ({SysInfo.MOBO.Manufacturer})");
                Console.WriteLine(tab + $"BIOS │ {SysInfo.MOBO.BIOS} [{SysInfo.MOBO.BIOSDate}]");
                Console.WriteLine(_div);
            }
            else
            {
                Console.WriteLine(tab + "MOBO │ UNKNOWN");
                Console.WriteLine(_div);
            }

            if (SysInfo.Drives != null)
            {
                bool _check = true;
                Console.Write(tab + "DISK ");
                foreach (Drive drive in SysInfo.Drives.List)
                {
                    if (drive.MediaType != "Removable Media")
                    {
                        if (_check)
                        {
                            _check = false;
                        }
                        else
                        {
                            Console.Write(tab + "     ");
                        }
                        Console.Write($"│ {drive.Name} ({drive.Size:N0} GB) [{drive.Partitions} Partitions]\n");
                    }
                }
                Console.WriteLine(_div);
            }
            else
            {
                Console.WriteLine(tab + "DISK │ UNKNOWN");
                Console.WriteLine(_div);
            }

            if (SysInfo.OS.ProdKey != null)
            {
                Console.WriteLine(tab + $"  OS │ {OS.Name}");
                Console.WriteLine(tab + $" KEY │ {OS.ProdKey}");
            }
            else
            {
                Console.WriteLine(tab + $"  OS │ {OS.Name}");
                Console.WriteLine(tab + $" KEY │ NOT ACTIVATED");
            }
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
    class CPUData
    {
        public string Name;
        public string Manufacturer;
        public string Description;

        public CPUData()
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
    class GPUData
    {
        public string Name;
        public string DriverVersion;
        public string DriverDate;

        public GPUData()
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
    class MOBOData
    {
        public string Name;
        public string Manufacturer;
        public string BIOS;
        public string BIOSDate;
        public string SerialNumber;

        public MOBOData()
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
    class RAMData
    {
        public long TotalCapacity;
        public long SingleCapacity;
        public int NumSticks;
        public int Speed;
        public string Name;
        public string Manufacturer;

        public RAMData()
        {
            string wmiObject = "Win32_PhysicalMemory";
            var searcher = new SysChecker(wmiObject);

            Name = searcher.GetFirst("PartNumber").Trim();
            Manufacturer = searcher.GetFirst("Manufacturer").Trim();
            Speed = Convert.ToInt32(searcher.GetFirst("Speed"));

            TotalCapacity = 0;
            NumSticks = 0;
            long _capacity = 0;
            foreach (string size in searcher.Get("Capacity"))
            {
                _capacity += Convert.ToInt64(size);
                NumSticks++;
            }
            TotalCapacity = _capacity / (1024 * 1024 * 1024);
            SingleCapacity = TotalCapacity / NumSticks;
        }
    }

    /// <summary>
    /// Stores physical disk information
    /// </summary>
    class DriveData
    {
        public List<Drive> List = new List<Drive>();

        public DriveData()
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


    class OSData
    {
        public string ProdKey;
        public string Name;

        public OSData()
        {
            string wmiObject = "Win32_OperatingSystem";
            var searcher = new SysChecker(wmiObject);

            Name = searcher.GetFirst("Caption");
        }

        public void GetProdKey()
        {
            ProdKey = WinKeyDecoder.CheckWindowsProductKey();
        }
    }
}
