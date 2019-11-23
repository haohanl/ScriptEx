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
            ExpandStageCommands();
            PrimeCommands();
            ExecuteCommands();
        }

        // Expand stages into commands
        static void ExpandStageCommands()
        {
            // Create an expanded command list.
            List<string> expandedCommands = new List<string>();
            foreach (string cmd in TerminalOld.userInput)
            {
                var tmp = Program.Config.GetStage(cmd);
                if (tmp != null)
                {
                    List<string> stage = new List<string>(tmp.Commands.Trim().Split(' '));
                    expandedCommands.AddRange(stage);
                }
                else
                {
                    expandedCommands.Add(cmd);
                }
            }

            // Replace old commands
            TerminalOld.userCommands = expandedCommands;
            TerminalOld.userInput = expandedCommands;
        }

        // Filter commandline arguments and execute RoboCopy
        static void PrimeCommands()
        {
            // filter all commands
            foreach (string cmd in TerminalOld.userInput)
            {
                AppConfigTest tmp = Program.Config.GetTest(cmd);

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
                    AppConfigTest test = Program.Config.GetTest(command);
                    Thread execThread = Testing.StartTest(test);

                    // Add to threadblock if specified
                    if (!test.IgnoreThreadBlock)
                    {
                        threadBlock.Add(execThread);
                    }
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
