using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;

namespace ScriptExDee_II
{
    public static class RoboCopy
    {
        // Remote drive source
        public static string SrcDrive;

        // Commandline constants
        private static readonly string Shell = "cmd.exe";
        private static readonly string Command = "ROBOCOPY";
        private static readonly string Params = @"/e /xo /nc /ns /np /NJH /NDL /NFL /MT";

        /// <summary>
        /// Search for remote source
        /// </summary>
        public static void Initialise()
        {
            // Initialise the remote drive root
            if (Program.Config.RoboCopy.ForceSrcDriveLetter)
            {
                SrcDrive = $"{Program.Config.RoboCopy.SrcDriveLetter}:\\" + Program.Config.RoboCopy.SrcDriveRoot;
            }
            else
            {
                string _srcDrive;
                string _driveName = Program.Config.RoboCopy.SrcDriveName;
                DriveType _driveType = Program.Config.RoboCopy.SrcDriveType;

                // Search for exact drive
                _srcDrive = GetDrive(_driveName, _driveType);

                // If no exact drive is found, fallback to a drive of the same type
                if (_srcDrive != null)
                {
                    Console.WriteLine($"Source '{Program.Config.RoboCopy.SrcDriveName}' located on '{_srcDrive}'.");
                    SrcDrive = _srcDrive;
                    return;
                }

                // otherwise return the drive
                _srcDrive = GetDrive(_driveType);

                // Return drive root directory if valid drive is found
                if (_srcDrive != null)
                {
                    Console.WriteLine($"Source '{Program.Config.RoboCopy.SrcDriveName}' not found. Defaulting to '{_srcDrive}'.");
                    SrcDrive = _srcDrive;
                    return;
                }

                // If no matching devices are present, default to forced drive letter
                Console.WriteLine($"Source '{Program.Config.RoboCopy.SrcDriveName}' not found. Program will only run local files.");
                SrcDrive = null;
                return;
            }
        }


        public static void Reinitialise()
        {
            string _srcDrive;
            string _driveName = Program.Config.RoboCopy.SrcDriveName;
            DriveType _driveType = Program.Config.RoboCopy.SrcDriveType;

            // Search for exact drive
            _srcDrive = GetDrive(_driveName, _driveType);

            // If no exact drive is found, fallback to a drive of the same type
            if (_srcDrive != null)
            {
                if (_srcDrive != SrcDrive)
                {
                    Terminal.WriteLine($"Source '{Program.Config.RoboCopy.SrcDriveName}' located on '{_srcDrive}'.", "*");
                    SrcDrive = _srcDrive;
                }
                return;
            }

            // otherwise return the drive
            _srcDrive = GetDrive(_driveType);

            // Return drive root directory if valid drive is found
            if (_srcDrive != null)
            {
                if (_srcDrive != SrcDrive)
                {
                    Terminal.WriteLine($"Source '{Program.Config.RoboCopy.SrcDriveName}' not found. Defaulting to '{_srcDrive}'.", "!");
                    SrcDrive = _srcDrive;
                }
                return;
            }

            // If no matching devices are present
            Terminal.WriteLine($"Source '{Program.Config.RoboCopy.SrcDriveName}' not found. Program will only run local files.", "!");
            SrcDrive = null;
            return;
        }

        /// <summary>
        /// Copy required files for a given command
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="key"></param>
        public static void Copy(string mode, string key)
        {
            // Get relevant config data
            CommandItem _command = Program.Config.GetCommandItem(mode, key);
            ModeConfig _mode = Program.Config.Modes[mode];

            // Check for Copy permission
            if (!_mode.SrcCopy)
            {
                return;
            }

            // Set transfer name
            string _desc = _command.Name;

            // Create paths for robocopy
            string _modeRoot = Path.Combine(SrcDrive, _mode.SrcModeRoot);
            string _srcPath = _command.GetNewestSrcPath(_modeRoot);

            string _dstRoot = Environment.ExpandEnvironmentVariables(_mode.DstModeRoot);
            string _dstPath = _command.GetDstPath(_dstRoot);

            // Validate paths
            if (_srcPath == null || _dstPath == null)
            {
                return;
            }

            // Execute copy
            Copy(_srcPath, _dstPath, _desc);

        }

        /// <summary>
        /// Initiate RoboCopy between two paths
        /// </summary>
        public static void Copy(string srcPath, string dstPath, string desc)
        {

            // Setup RoboCopy parameters
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = Shell,
                Arguments = $@"/C {Command} ""{srcPath}"" ""{dstPath}"" {Params}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            //proc.StartInfo.WorkingDirectory = Program.RootPath;

            // Start RoboCopy
            Process proc = new Process
            {
                StartInfo = info
            };
            proc.Start();
            Terminal.WriteLine($"Initiated RoboCopy | {desc}", "*");

            // Catch RoboCopy log
            string log = proc.StandardOutput.ReadToEnd().Trim();

            // Extract transfer data
            var transfer = Regex.Match(log, @"(\d+\.\d+ [mgkt])").Groups;

            // Check for errors. If found, output raw log
            if (transfer[0].Value == "")
            {
                Terminal.WriteLine(log, "!");
            }
            // Otherwise, output size info
            else
            {
                Terminal.WriteLine($"Completed Robocopy | {desc} | {transfer[0].Value.ToUpper() + "B"}", "*");
            }

            // Terminate proc
            proc.WaitForExit();
            proc.Close();
        }

        /// <summary>
        /// Search the system for a matching drive given a drive type.
        /// </summary>
        static string GetDrive(DriveType srcDriveType)
        {
            // Root path
            string _root = Program.Config.RoboCopy.SrcDriveRoot;

            // Filter drives to those that match the drive type
            var _drives = from drive in DriveInfo.GetDrives()
                          where drive.DriveType == srcDriveType
                          select drive;

            if (_drives.Count() > 0)
            {
                return _drives.First().RootDirectory.ToString() + _root;
            }

            return null;
        }

        /// <summary>
        /// Search the system for a matching drive given drive name and drive type.
        /// </summary>
        static string GetDrive(string srcDriveName, DriveType srcDriveType)
        {
            // Root path
            string _root = Program.Config.RoboCopy.SrcDriveRoot;

            // Filter drives to those that match the drive type
            var _drives = from drive in DriveInfo.GetDrives()
                          where drive.DriveType == srcDriveType && drive.VolumeLabel == srcDriveName
                          select drive;

            // Return drive if found
            if (_drives.Count() > 0)
            {
                return _drives.First().RootDirectory.ToString() + _root;
            }

            return null;
        }
    }
}
