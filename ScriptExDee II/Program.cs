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
        public static string Version = "20.02.08";
        public static string Title = "ScriptExDee II";
        public static string Quote = Quotes.GetQuote();
        public static string ExecPath = AppDomain.CurrentDomain.BaseDirectory;

        // Configuration file
        public static string ConfigFile = "AppConfig.yml";
        public static Config Config = null;


        static void Main()
        {
            InitialiseConfig();

            ExitHandler.Start();

            Shelleton.Shell.Initialise();

            InitialiseTitleScreen();

            Terminal.Start();
        }

        static void InitialiseConfig()
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
                Terminal.WriteLineBreak();
                Console.WriteLine($"'{ConfigFile}' could not be loaded. Program unable to start.");
                Terminal.WriteLineBreak();

                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);

                Terminal.WriteLineBreak();
                Console.WriteLine("\nProgram will exit...");
                Console.ReadKey();
                Environment.Exit(-1);
            }
        }

        static void InitialiseTitleScreen()
        {
            TitleScreen.ShowTitle();
            Shelleton.Shell.Run(Config.Program.StartupCommands);

            TitleScreen.ShowSummary();
        }
    }
}
