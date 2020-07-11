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
        public static string Version = "20.07.11"; // Dont forget to update AssemblyInfo.cs
        public static string Title = "ScriptExDee II";
        public static string Quote = Quotes.GetQuote();
        public static string ExecPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        public static string ExecFolder = AppDomain.CurrentDomain.BaseDirectory;

        // Configuration file
        public static string ConfigFile = "AppConfig.yml";
        public static Config Config = null;


        static void Main()
        {
            // Redirect unhandled exception to custom handler
            // See: https://stackoverflow.com/questions/31366174/enable-console-window-to-show-exception-error-details
            if (!System.Diagnostics.Debugger.IsAttached) {
                AppDomain.CurrentDomain.UnhandledException += ReportUnhandledException;
            }

            // Initialise
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

        private static void ReportUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Console.WriteLine();
            Console.WriteLine(new String('=', 119));
            Console.WriteLine("SCRIPTEX HAS CRASHED. SHOW SOMEONE THE LOGS BELOW.");
            Console.WriteLine(new String('=', 119));
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine(new String('-', 119));
            Console.WriteLine("Please press any key to end the program...");
            Console.WriteLine(new String('=', 119));
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
