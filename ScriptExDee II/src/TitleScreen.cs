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
{HTAB}   _____           _       __  ______     ____               ________
{HTAB}  / ___/__________(_)___  / /_/ ____/  __/ __ \___  ___     /  _/  _/
{HTAB}  \__ \/ ___/ ___/ / __ \/ __/ __/ | |/_/ / / / _ \/ _ \    / / / /  
{HTAB} ___/ / /__/ /  / / /_/ / /_/ /____>  </ /_/ /  __/  __/  _/ /_/ /   
{HTAB}/____/\___/_/  /_/ .___/\__/_____/_/|_/_____/\___/\___/  /___/___/   
{HTAB}                /_/ {Program.Quote} 
{TAB}Haohan Liu (c) 2020";

        public const string LTAB = "";
        public const string TAB = "        ";
        const int TSIZE = Terminal.TSIZE;
        const string HTAB = "                       ";

        public static void ShowTitle()
        {          
            // Animate starting text
            AnimateWrite(Title, Program.Config.Program.TitleScreenDelay);
            WriteLineBreak('═', TSIZE);
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

            //

            Terminal.WriteLineBreak();

            // Show SysSummary
            SysInfo.SysSummary(TAB);
            Terminal.WriteLineBreak();

            // Begin RoboCopy initialisation
            Console.Write(TAB);
            RoboCopy.Initialise();
            Terminal.WriteLineBreak();

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
