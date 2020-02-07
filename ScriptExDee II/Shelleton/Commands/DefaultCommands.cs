using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ScriptExDee_II.Shelleton.Commands
{
    public static class DefaultCommands
    {

        #region # Terminal accessible commands
        public static void h()
        {
            Console.WriteLine("SHELLETON COMMANDS (<> Required argument, [] Optional argument):");
            Help();
        }

        public static void si()
        {
            SysSummary();
        }

        public static void cleanup(bool toggle=true)
        {
            DeleteTaskService();
            CleanupOnExit(toggle);
        }

        public static void autostart(bool toggle=true)
        {
            if (toggle)
            {
                CreateTaskService();
            }
            else
            {
                DeleteTaskService();
            }
        }

        public static void checkpath(string path)
        {
            if (Directory.Exists(path))
            {
                Console.WriteLine($"Path exists [{path}]");
            }
            else
            {
                Console.WriteLine($"Path not found [{path}]");
            }
        }

        /// <summary>
        /// Retrieve CopyTransferInformation from a specific command key
        /// </summary>
        public static void cti(string mode, string key)
        {
            CommandTransferInfo _cti = new CommandTransferInfo(mode, key);
            Console.WriteLine("NAME: " + _cti.Name);
            Console.WriteLine("NEWEST: " + _cti.NewestSrcPath);
            Console.WriteLine("SRC: " + _cti.SrcPath);
            Console.WriteLine("NET: " + _cti.NetPath);
            Console.WriteLine("DST: " + _cti.DstPath);
        }
        #endregion


        #region # Program interface methods
        static void Help()
        {
            foreach (var key in Shell.GetCommands())
            {
                Console.WriteLine(" - " + key);
            }
        }
        
        static void SysSummary()
        {
            SysInfo.Initialise();
            Terminal.WriteLineBreak();
            SysInfo.SysSummary(" ");
        }

        static void CreateTaskService()
        {
            TaskHandler.CreateTaskService();
        }

        static void DeleteTaskService()
        {
            TaskHandler.DeleteTaskService();
        }

        static void CleanupOnExit(bool toggle=true)
        {
            ExitHandler.CleanupOnExit(toggle);
        }
        #endregion
    }
}
