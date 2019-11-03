using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ScriptExDee
{
    static class TitleScreen
    {
        public static string Title = $@"
         _____           _       __  ______     ____          
        / ___/__________(_)___  / /_/ ____/  __/ __ \___  ___ 
        \__ \/ ___/ ___/ / __ \/ __/ __/ | |/_/ / / / _ \/ _ \
       ___/ / /__/ /  / / /_/ / /_/ /____>  </ /_/ /  __/  __/
      /____/\___/_/  /_/ .___/\__/_____/_/|_/_____/\___/\___/ 
                      /_/  [Build {Program.Version}]                                   

      Written by Haohan Liu (c) 2019          
";
        public static void ShowTitle()
        {
            Console.Write(Title);
            Console.Title = $"{Program.Title} [Build {Program.Version}]";
            Console.WriteLine("      \n      --------------------------------------------------------\n");
            Thread.Sleep(100);
            Console.Write("      Importing 'AppConfig.xml'...");
            Thread.Sleep(200);
            Console.WriteLine(" done.");

            Thread.Sleep(100);
            Console.Write("      Initialising Software Suite...");
            Thread.Sleep(50);
            Console.WriteLine(" done.");

            Thread.Sleep(100);
            Console.Write("      Initialising Testing Suite...");
            Thread.Sleep(50);
            Console.WriteLine(" done.");

            Thread.Sleep(100);
            Console.Write("      Set Win10 to Performance Mode...");
            PowerControl.SetToPerformance();
            Console.WriteLine(" done.");

            Console.WriteLine("      \n      --------------------------------------------------------\n");
            Console.Write("      Ready to roll...");

            Thread.Sleep(1000);
            Console.Clear();
        }
    }

}
