using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ScriptExDee_II
{
    class TitleScreen
    {
        // Source: http://patorjk.com/software/taag/#p=display&f=Slant&t=ScriptExDee%20II
        static readonly string Title = $@"
          _____           _       __  ______     ____               ________
         / ___/__________(_)___  / /_/ ____/  __/ __ \___  ___     /  _/  _/
         \__ \/ ___/ ___/ / __ \/ __/ __/ | |/_/ / / / _ \/ _ \    / / / /  
        ___/ / /__/ /  / / /_/ / /_/ /____>  </ /_/ /  __/  __/  _/ /_/ /   
       /____/\___/_/  /_/ .___/\__/_____/_/|_/_____/\___/\___/  /___/___/   
                       /_/ {Program.Quote} 
       Haohan Liu (c) 2020";

        public const string LTAB = "     ";
        public const string TAB = "       ";

        public static void ShowTitle()
        {          
            // Animate starting text
            AnimateWrite(Title, Program.Config.Program.TitleScreenDelay);
            WriteLineBreak('=', 75);
        }

        public static void Show()
        {
            // Start Threads
            Program.ThrShowTitle.Start();
            Program.ThrPowerControl.Start();

            // Check for System check parameter
            if (!Program.Config.Program.DisableSystemCheck)
            {
                Program.ThrSysInfo.Start();
                Program.ThrSysInfo.Join();
            }

            // Join threads
            Program.ThrShowTitle.Join();
            Program.ThrPowerControl.Join();

            // Show SysSummary
            SysInfo.SysSummary(TitleScreen.TAB);
            WriteLineBreak('-', 75);

            // Begin RoboCopy initialisation
            Console.Write(TitleScreen.TAB);
            RoboCopy.Initialise();
            WriteLineBreak('-', 75);

            // Take user key
            Console.Write(TAB + "Press any key to begin...");
            ReadModeKey();
            Console.Clear();
        }

        static void AnimateWrite(string title, int delay)
        {
            string[] tmp = title.Split('\n');
            foreach (string line in tmp)
            {
                Console.WriteLine(line);
                Thread.Sleep(delay);
            }
        }

        public static void ReadModeKey()
        {
            var _input = Console.ReadKey();

            string _modekey = "!" + _input.KeyChar;

            // check for mode
            if (Program.Config.IsModeKey(_modekey))
            {
                Terminal.CurrentMode = Program.Config.GetMode(_modekey);
            }
            else
            {
                Terminal.CurrentMode = Program.Config.GetMode(Program.Config.Program.DefaultMode);
            }
        }

        public static void WriteLine(string line="")
        {
            Console.WriteLine(TAB + line);
        }

        public static void WriteLineBreak(char c, int len)
        {
            Console.WriteLine(LTAB + new String(c, len));
        }
    }
}
