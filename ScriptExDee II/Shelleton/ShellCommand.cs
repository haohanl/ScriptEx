using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ScriptExDee_II.Shelleton
{
    public class ShellCommand
    {
        // Class attributes
        public string Name { get; set; }
        public string LibraryClassName { get; set; }

        public List<string> Arguments;

        // Regex
        const string UnquotedSpaces = "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
        private static readonly Regex QuotedString = new Regex("\"(.*?)\"", RegexOptions.Singleline);
        private static readonly Regex UnquotedString = new Regex("[^\"]*[^\"]");


        // ConsoleCommand constructor
        public ShellCommand(string input)
        {
            string[] _inputArray = Regex.Split(input, UnquotedSpaces);

            Arguments = new List<string>();
            for (int i = 0; i < _inputArray.Length; i++)
            {
                // First element is always the command
                if (i == 0)
                {
                    // Set the default
                    this.Name = _inputArray[i];
                    this.LibraryClassName = "DefaultCommands";

                    // Check for other origins
                    string[] _s = _inputArray[0].Split('.');
                    if (_s.Length > 1)
                    {
                        this.Name = _s.Last();
                        Array.Resize(ref _s, _s.Count() - 1);
                        this.LibraryClassName = string.Join(".", _s);
                    }
                }

                // Filter arguments
                else
                {
                    string _input = _inputArray[i];
                    Arguments.Add(CaptureQuotedString(_input));
                }
            }
        }

        private string CaptureQuotedString(string input)
        {
            // Match quoted string
            Match _match = QuotedString.Match(input);
            if (_match.Captures.Count > 0)
            {
                // Capture unquoted string
                Match _unquoted = UnquotedString.Match(_match.Captures[0].Value);
                return _unquoted.Captures[0].Value;
            }
            return input;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // Output command details
            sb.AppendLine(string.Format("Executed command: {0}.{1}", this.LibraryClassName, this.Name));

            // Output command arguments
            for (int i = 0; i < this.Arguments.Count(); i++)
            {
                sb.AppendLine(string.Format("Argument {0}: {1}", i, this.Arguments[i]));
            }

            return sb.ToString();
        }
    }
}
