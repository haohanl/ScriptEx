using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ScriptExDee
{
    static class TestingMode
    {
        public static void RunCommands()
        {
            PrimeCommands();
            ExecuteCommands();
        }

        // Filter commandline arguments and execute RoboCopy
        static void PrimeCommands()
        {
            // filter all commands
            foreach (string cmd in Terminal.userInput)
            {
                AppConfigTest tmp = Program.Config.GetTest(cmd);

                // Check for special characters
                switch (cmd)
                {
                    case Terminal.ThreadBreakKey:
                        break;

                    // If none, check for commands
                    default:
                        switch (tmp)
                        {
                            // Remove invalid command from execution process
                            case null:
                                Terminal.WriteLine($"'{cmd}' is not a valid command. Ignoring.", "!");
                                Terminal.userCommands.Remove(cmd);
                                break;
                            // Add valid command to execution process
                            default:
                                break;
                        }
                        break;
                }
            }
        }

        // execute script installers according to thread blocks
        static void ExecuteCommands()
        {
            List<Thread> threadBlock = new List<Thread>();

            foreach (var command in Terminal.userCommands)
            {
                // Check for thread block
                if (command == Terminal.ThreadBreakKey)
                {
                    // Terminal Output
                    Terminal.WriteLine("Start thread batch.", "|");

                    // Trigger block
                    foreach (var thr in threadBlock)
                    {
                        thr.Join();
                    }
                    // Reset block
                    threadBlock = new List<Thread>();
                    // Terminal Output
                    Terminal.WriteLine("Ended thread batch.", "|");
                }
                else
                {
                    // Grab script and begin execution
                    AppConfigTest test = Program.Config.GetTest(command);
                    threadBlock.Add(Testing.StartTest(test));
                }
            }

            // Trigger end of chain block
            foreach (var thr in threadBlock)
            {
                thr.Join();
            }
        }
    }
}
