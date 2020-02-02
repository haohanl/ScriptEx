using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExDee_II
{
    class Program
    {
        // Program information
        public static string Version = "20.02.02";
        public static string Title = "ScriptExDee";
        public static string Quote = Quotes.GetQuote();

        // Configuration file
        public static string ConfigFile = "AppConfig.yml";
        public static Config Config = null;

        static void Main(string[] args)
        {
            try
            {
                Config = YAMLHandler.Deserialize(ConfigFile);
                //Console.WriteLine(String.Join("\n\n", _config.Modes["Testing"].Commands));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("Config Loaded...");

            Terminal.Start();



            Console.WriteLine("Program has ended...");
            Console.ReadKey();
        }
    }
}
