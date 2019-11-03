using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace ScriptExDee
{
    public static class DMChecker
    {
        private const int DRAM_SPEED_LOW_THRESH = 2000;   // Due to AMD RAM speeds being inconsistent (reporting half speed only occasionally),
                                                          // RAM speeds below this threshold will me multiplied by 2.

        public static string GetGPUName()
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity WHERE PNPClass = 'Display'"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                devices.Add(new DeviceInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("PNPDeviceID"),
                (string)device.GetPropertyValue("Description"),
                (string)device.GetPropertyValue("Status")
                ));
            }

            collection.Dispose();
            return devices.First().Description;
        }

        public static string GetGPUDriver()
        {
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_PnPSignedDriver WHERE DeviceClass = 'Display'");

            ManagementObjectCollection objectCollection = objSearcher.Get();

            string driver = "";

            foreach (ManagementObject obj in objectCollection)
            {
                driver = String.Format("{0}", obj["DriverVersion"]);
            }

            return driver;
        }

        public static string GetCPUName()
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity WHERE PNPClass = 'Processor'"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                devices.Add(new DeviceInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("PNPDeviceID"),
                (string)device.GetPropertyValue("Description"),
                (string)device.GetPropertyValue("Status")
                ));
            }

            collection.Dispose();
            return devices.First().DeviceID;
        }

        public static bool GetStatus()
        {
            var devices = GetDevices();

            foreach (var device in devices)
            {
                if (device.Status != "OK")
                {
                    return false;
                }
            }
            return true;
        }

        private static List<DeviceInfo> GetDevices()
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                devices.Add(new DeviceInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("PNPDeviceID"),
                (string)device.GetPropertyValue("Description"),
                (string)device.GetPropertyValue("Status")
                ));

            }

            collection.Dispose();
            return devices;
        }

        public static void GetRAMDescription(ref int size, ref int speed)
        {
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_PhysicalMemory");

            ManagementObjectCollection objectCollection = objSearcher.Get();

            //string description = "";
            int totalCapacity = 0;
            long capacity = 0;
            speed = 0;

            try
            {
                foreach (ManagementObject obj in objectCollection)
                {
                    string caps = obj["Capacity"].ToString();
                    Int64.TryParse(obj["Capacity"].ToString(), out capacity);
                    totalCapacity += (int)(capacity / (1024 * 1024 * 1024));
                    Int32.TryParse(obj["ConfiguredClockSpeed"].ToString(), out speed);
                    if (SysInfo.CPUBrand == CPUBrand.AMD && (speed < DRAM_SPEED_LOW_THRESH))
                    {
                        speed *= 2;   // On AMD systems, the reported speed is half the actual setting
                    }
                    //description= String.Format("{0}GB @{1}MHz", totalCapacity, obj["ConfiguredClockSpeed"]); 
                }
            }
            catch
            {
                // ADD LOGGING SOMETIME
            }
            size = totalCapacity;
        }
    }

    class DeviceInfo
    {
        public DeviceInfo(string deviceID, string pnpDeviceID, string description, string status)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
            this.Status = status;
            this.Driver = "";
        }

        public DeviceInfo(string deviceID, string pnpDeviceID, string description, string status, string driver)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
            this.Status = status;
            this.Driver = driver;
        }

        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }
        public string Status { get; private set; }
        public string Driver { get; private set; }
    }
}
