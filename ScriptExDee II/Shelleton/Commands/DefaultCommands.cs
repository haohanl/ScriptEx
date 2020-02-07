using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
