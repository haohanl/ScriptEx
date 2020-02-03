using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ScriptExDee_II
{
    class Program
    {
        // Program information
        public static string Version = "20.02.04";
        public static string Title = "ScriptExDee II";
        public static string Quote = Quotes.GetQuote();

        // Configuration file
        public static string ConfigFile = "AppConfig.yml";
        public static Config Config = null;

        // Program background threads
        public static Thread ThrSysInfo = new Thread(SysInfo.GatherSysInfo);
        public static Thread ThrShowTitle = new Thread(TitleScreen.ShowTitle);

        static void Main(string[] args)
        {
            Initialise();

            TitleScreen.Show();
            
            Terminal.Start();
        }

        static void Initialise()
        {
            Terminal.Theme.ApplyTheme();
            Terminal.Title.Update();

            // Load system config
            try
            {
                Config = YAMLHandler.Deserialize(ConfigFile);
            }
            catch (Exception ex)
            {
                Thread.Sleep(1000);
                Terminal.WriteLineBreak('-', 80);
                Console.WriteLine($"'{ConfigFile}' could not be loaded. Program unable to start.");
                Terminal.WriteLineBreak('-', 80);
                Console.WriteLine(ex);
                Terminal.WriteLineBreak('-', 80);
                Console.WriteLine($"'{ConfigFile}' could not be loaded. Program unable to start.");
                Terminal.WriteLineBreak('-', 80);
                Console.WriteLine("\nProgram will exit...");
                Console.ReadKey();
            }
        }
    }
}
