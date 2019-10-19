using System;
using System.IO;
using System.Threading;

namespace ScriptEx
{
    class Program
    {
        const int majVer = 1;
        const int minVer = 0;
        const int subVer = 0;

        const string cmdConfig = "cmd.xml";

        static void Main(string[] args)
        {
            // CLI mode
            if (args.Length == 0)
            {
                // Header
                HRule('=');
                Console.WriteLine("ScriptEx " + $"[{majVer}.{minVer}.{subVer}] | Don't worry, it's not sketchy at all.");
                HRule();
                Console.WriteLine("ROOT: " + Installer.Root);
                HRule('=');

                // XML Intake
                XMLHandler xmlConfig;
                try
                {
                    xmlConfig = new XMLHandler(cmdConfig);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"[-] ERR: {cmdConfig} not found. Application terminating.");
                    Console.ReadKey();
                    return;
                }
                
                // Basic interface loop
                string userInput = "";
                string shellCommand;
                int exeCount, cmdCount;

                while (userInput.ToLower().Trim() != "q")
                {
                    // Reset counter
                    cmdCount = 0;

                    // Take user input
                    //Console.Write("> ");
                    userInput = Console.ReadLine();
                    string[] userCmds = userInput.Trim().Split(' ');

                    // Tally command requests
                    HRule();
                    exeCount = userCmds.Length;
                    Console.WriteLine($"{exeCount} command(s) recieved.");
                    HRule();

                    // Process each command
                    foreach (string cmd in userCmds)
                    {
                        string[] commandArray = xmlConfig.Get(cmd);
                        shellCommand = string.Join(" ", commandArray).Trim(); 

                        // Early termination
                        if (cmd == "q")
                        {
                            Console.WriteLine($"[{cmdCount}] 'q' \t| Quit request received. Terminating input.");
                            userInput = "q";
                            break;
                        }
                        else if (shellCommand == "")
                        {
                            Console.WriteLine($"[{cmdCount}] '{cmd}' \t| Unknown command. Skipped.");
                        }
                        else
                        {
                            Console.WriteLine($"[{cmdCount}] '{cmd}' \t| Executing.");
                            Installer.RunThread(commandArray[0], commandArray[1]);
                        }

                        cmdCount++;
                    }
                    Console.WriteLine();
                }


                HRule();
                Console.WriteLine("Main() thread terminated. Program will exit when other threads end.");
                HRule();
            }

            // Param mode
            else
            {
                Console.WriteLine("To be implemented.");
            }
        }

        static void HRule(char c = '-')
        {
            Console.WriteLine(new String(c, 80));
        }
    }
}
