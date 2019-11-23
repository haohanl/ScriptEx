using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ScriptExDee
{
    static class SoftwareMode
    {

        public static void RunCommands()
        {
            PrimeCommands();
            ExecuteCommands();
        }


        static void PrimeCommands()
        {
            // reset installCommands
            List<AppConfigScript> validCommands = new List<AppConfigScript>();

            // filter all commands
            foreach (string cmd in TerminalOld.userInput)
            {
                AppConfigScript tmp = Program.Config.GetScript(cmd);

                // Check for special characters
                switch (cmd)
                {
                    case TerminalOld.ThreadBreakKey:
                        break;

                    // If none, check for commands
                    default:
                        switch (tmp)
                        {
                            // Remove invalid command from execution process
                            case null:
                                TerminalOld.WriteLine($"'{cmd}' is not a valid command. Ignoring.", "!");
                                TerminalOld.userCommands.Remove(cmd);
                                break;
                            // Add valid command to execution process
                            default:
                                validCommands.Add(tmp);
                                break;
                        }
                        break;
                }
            }

            // RoboCopy valid commands
            RoboCopy.BatchCopy(validCommands);
        }


        // execute script installers according to thread blocks
        static void ExecuteCommands()
        {
            List<Thread> threadBlock = new List<Thread>();

            foreach (var command in TerminalOld.userCommands)
            {
                // Check for thread block
                if (command == TerminalOld.ThreadBreakKey)
                {
                    // Terminal Output
                    TerminalOld.WriteLine("Start thread batch.", "|");

                    // Trigger block
                    foreach (var thr in threadBlock)
                    {
                        thr.Join();
                    }
                    // Reset block
                    threadBlock = new List<Thread>();
                    // Terminal Output
                    TerminalOld.WriteLine("Ended thread batch.", "|");
                }
                else
                {
                    // Grab script and begin execution
                    AppConfigScript script = Program.Config.GetScript(command);
                    threadBlock.Add(Installer.StartInstall(script));
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
