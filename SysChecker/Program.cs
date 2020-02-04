using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;

namespace SysChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("System WMI Data Extraction");
            Console.Write("Output Filename: ");
            string name = Console.ReadLine();

            string[] wmiArray = new string[]
            {
                "Win32_Processor",
                "Win32_VideoController",
                "Win32_DisplayConfiguration",
                "Win32_BaseBoard",
                "Win32_BIOS",
                "Win32_PhysicalMemory",
                "Win32_PhysicalMedia",
                "Win32_DiskDrive",
                "Win32_LogicalDisk",
                "Win32_Volume",
                "Win32_OperatingSystem"
            };

            using (StreamWriter writer = new StreamWriter(name + ".txt"))
            {
                foreach (var wmi in wmiArray)
                {
                    writer.WriteLine("-----------------------------------------------");
                    writer.WriteLine(wmi);
                    writer.WriteLine("-----------------------------------------------");
                    foreach (var item in GetPropertiesOfWmiClass("root\\cimv2", wmi))
                    {
                        writer.WriteLine(item);
                    }
                    writer.WriteLine();
                    Console.WriteLine($"{wmi} done...");
                }
            }
            //CPU DATA
            //PrintPropertiesOfWmiClass("root\\cimv2", "Win32_Processor");

            //GPU DATA
            //PrintPropertiesOfWmiClass("root\\cimv2", "Win32_VideoController");

            //MOBO DATA, Manufacturer, Product
            //PrintPropertiesOfWmiClass("root\\cimv2", "Win32_BaseBoard");

            //BIOS DATA, Name, ReleaseDate
            //PrintPropertiesOfWmiClass("root\\cimv2", "Win32_BIOS");

            // HDD SERIAL
            //PrintPropertiesOfWmiClass("root\\cimv2", "Win32_PhysicalMedia");

            //PrintPropertiesOfWmiClass("root\\cimv2", "Win32_NetworkAdapter");
            //GetCPUName();

            Console.WriteLine("EOP, Press any key to exit...");

            Console.ReadKey();
        }

        public static void GetCPUName()
        {
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity WHERE Manufacturer='Samsung'"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                Console.WriteLine("HIT");
            }
        }

        //https://dotnetcodr.com/2014/11/25/finding-all-wmi-class-properties-with-net-c/
        private static List<string> GetPropertiesOfWmiClass(string namespaceName, string wmiClassName)
        {
            List<string> propVals = new List<string>();

            ManagementPath managementPath = new ManagementPath();
            managementPath.Path = namespaceName;
            ManagementScope managementScope = new ManagementScope(managementPath);
            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM " + wmiClassName);
            ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(managementScope, objectQuery);
            ManagementObjectCollection objectCollection = objectSearcher.Get();
            foreach (ManagementObject managementObject in objectCollection)
            {
                PropertyDataCollection props = managementObject.Properties;
                foreach (PropertyData prop in props)
                {
                    propVals.Add($"{prop.Name}: {prop.Value} [{prop.Type}]");
                }
                propVals.Add("");
            }

            return propVals;
        }

        //https://dotnetcodr.com/2014/11/25/finding-all-wmi-class-properties-with-net-c/
        private static void PrintPropertiesOfWmiClass(string namespaceName, string wmiClassName)
        {
            ManagementPath managementPath = new ManagementPath();
            managementPath.Path = namespaceName;
            ManagementScope managementScope = new ManagementScope(managementPath);
            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM " + wmiClassName);
            ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(managementScope, objectQuery);
            ManagementObjectCollection objectCollection = objectSearcher.Get();
            foreach (ManagementObject managementObject in objectCollection)
            {
                PropertyDataCollection props = managementObject.Properties;
                foreach (PropertyData prop in props)
                {
                    Console.WriteLine($"{prop.Name}: {prop.Value} [{prop.Type}]");
                }
                Console.WriteLine();
            }
        }
    }
}
