using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExDee_II.Shelleton.Commands
{
    class Startup
    {
        public static void RunSystemCheck(bool toggle=true)
        {
            if (toggle)
            {
                Console.Write(TitleScreen.TAB + "Run System Check... ");
                SysInfo.Initialise();
                Console.WriteLine("done");
            }
            else
            {
                Console.WriteLine(TitleScreen.TAB + "Run System Check... disabled");
            }
        }

        public static void AutoWinUpdate(bool toggle=true)
        {
            // TO BE IMPLEMENTED
            if (toggle)
            {
                //
            }
            else
            {
                //
            }
        }

        public static void SetPerformanceMode(bool toggle=true)
        {
            if (toggle)
            {
                Console.Write(TitleScreen.TAB + "Set to Performance Mode... ");
                PowerControl.SetToPerformance();
                Console.WriteLine("done");
            }
            else
            {
                Console.WriteLine(TitleScreen.TAB + "Set to Performance Mode... disabled");
            }
        }

        public static void EnableAutoStart(bool toggle=true)
        {
            if (toggle)
            {
                Console.Write(TitleScreen.TAB + "Enable Autostart Task... ");
                TaskHandler.CreateTaskService();
                Console.WriteLine("done");
            }
            else
            {
                Console.WriteLine(TitleScreen.TAB + "Enable Autostart Task... disabled");
            }
        }

        public static void DisableConsoleQuickEdit(bool toggle=true)
        {
            if (toggle)
            {
                Terminal.DisableConsoleQuickEdit.Go();
            }
        }
    }
}
