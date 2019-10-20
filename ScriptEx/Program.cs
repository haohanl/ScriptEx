using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace ScriptEx
{
    class Program
    {
        /// <summary>
        /// Entry point for ScriptEx
        /// </summary>
        /// <param name="args">
        /// To be implemented.
        /// </param>
        static void Main(string[] args)
        {
            // CLI mode
            if (args.Length == 0)
            {
                Terminal.Start();
            }

            // Param mode
            else
            {
                Console.WriteLine("To be implemented.");
            }
        }
    }
}
