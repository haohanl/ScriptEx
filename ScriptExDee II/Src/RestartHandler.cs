using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace ScriptExDee_II
{
    public static class RestartHandler
    {
        #region Restart commands
        static bool AwaitingRestart = false;
        public static void AwaitRestart()
        {
            Thread thr = new Thread(RestartWhenReady);
            thr.Start();
        }

        static void RestartWhenReady()
        {
            while (true)
            {
                if (AwaitingRestart)
                {
                    break;
                }
                Thread.Sleep(2000);
            }
            RestartSystem();
        }

        public static void AwaitingRestartState(bool toggle)
        {
            AwaitingRestart = toggle;
        }

        public static void RestartSystem()
        {
            Process.Start("shutdown", "/r /t 0");
        }

        #endregion
    }
}
