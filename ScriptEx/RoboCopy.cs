using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace ScriptEx
{
    public static class RoboCopy
    {
        // Constants
        private static readonly string Shell = "cmd.exe";
        private static readonly string Command = "ROBOCOPY";
        private static readonly string Parameters = @"/e /nc /ns /np /NJH /NDL /NFL /MT";

        // Execute RoboCopy
        public static void Run(Command cmd)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();

            proc.StartInfo.FileName = Shell;
            proc.StartInfo.Arguments = $@"/C {Command} ""{Program.InstallDir + cmd.SourcePath}"" ""{Program.CopyDestDir + cmd.DestPath}"" {Parameters}";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WorkingDirectory = Program.Root;

            proc.Start();
            Terminal.WriteLine("v", $"'{cmd.AttrVal}' | initialise RoboCopy");

            string output = proc.StandardOutput.ReadToEnd();
            Terminal.WriteLine("-", output);

            proc.WaitForExit();
            Terminal.WriteLine("^", $"'{cmd.AttrVal}' | completed RoboCopy");
            proc.Close();
        }

        public static Thread RunThread(Command cmd)
        {
            ThreadStart stThr = () => Run(cmd);
            Thread thr = new Thread(stThr);
            thr.Start();

            return thr;
        }

        public static void CheckCommand(Command cmd)
        {
            Console.WriteLine($@"/C {Command} ""{Program.InstallDir + cmd.SourcePath}"" ""{Program.CopyDestDir}"" {Parameters}");
        }
    }
}
