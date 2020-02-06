using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExDee_II.Shelleton.Commands
{
    public static class DefaultCommands
    {
        public static void Help()
        {
            foreach (var key in Shell.GetCommands())
            {
                Terminal.WriteLine(key);
            }
        }
        
        public static void SysSummary()
        {
            SysInfo.Initialise();
            Terminal.WriteLineBreak();
            SysInfo.SysSummary(" ");
        }

        public static void CreateTaskService()
        {
            TaskHandler.CreateTaskService();
        }

        public static void DeleteTaskService()
        {
            TaskHandler.DeleteTaskService();
        }

        public static void CleanupOnExit(bool toggle=true)
        {
            ExitHandler.CleanupOnExit(toggle);
        }
    }
}
