using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExDee
{
    public enum CPUBrand
    {
        Intel,
        AMD
    }

    public enum GPUBrand
    {
        nVIDIA,
        AMD,
        Unknown
    }

    public static class SysInfo
    {
        public static GPUInfo GPU;
        public static CPUInfo CPU;
        public static RAMInfo RAM;
        public static CPUBrand CPUBrand;

        // This method must be called before accessing fields
        public static void RetrieveSystemInfo()
        {
            GPU = new GPUInfo(DMChecker.GetGPUName(), DMChecker.GetGPUDriver());
            CPU = new CPUInfo(DMChecker.GetCPUName());
            RAM = new RAMInfo();   // Must be called after cpu (CpuBrand must be set)
        }

        public class GPUInfo
        {
            private string _name;
            private string _driver;
            private GPUBrand _gpuBrand = GPUBrand.Unknown;

            /// <summary>
            /// Instantiates a GPU in the system, with name, driver and brand information.
            /// </summary>
            /// <param name="gpuname">The raw GPU name.</param>
            /// <param name="gpudriver">The raw driver name.</param>
            public GPUInfo(string gpuname, string gpudriver)
            {
                string family = "";
                string model = "";
                string edition = "";
                _name = "";
                _driver = "";

                string[] words = gpuname.Split(' ');
                try
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i].Equals("GeForce") || words[i].Equals("Radeon"))
                        {
                            family = words[i + 1];
                            model = words[i + 2];
                            if (words.Length > (i + 3))
                            {
                                edition = words[i + 3];
                            }
                            if (words[i].Equals("GeForce")) { _gpuBrand = GPUBrand.nVIDIA; }
                            if (words[i].Equals("Radeon")) { _gpuBrand = GPUBrand.AMD; }
                            break;
                        }
                    }
                    _name = family + " " + model + " " + edition;

                    switch (_gpuBrand)
                    {
                        case GPUBrand.nVIDIA:
                            string[] numbersn = gpudriver.Split('.');
                            _driver = numbersn[2].Substring(1) + numbersn[3].Substring(0, 2) + "." + numbersn[3].Substring(2, 2);
                            break;
                        case GPUBrand.AMD:
                            string[] numbersa = gpudriver.Split('.');
                            _driver = numbersa[2].Substring(4, 1) + numbersa[3].Substring(0, 1) + ".xx";   // NOT complete
                            break;
                        default:
                            _driver = " ";
                            break;
                    }
                }
                catch { }

            }

            public string Name { get => _name; }
            public string Driver { get => _driver; }

            /// <summary>
            /// NOT IMPLEMENTED. Evaluates the given Heaven score against this GPU Model.
            /// </summary>
            /// <param name="score">The Heaven score.</param>
            /// <returns>Whether heaven score is acceptable.</returns>
            public bool HeavenScoreValidation(int score)
            {
                return true;
            }
        }

        public class CPUInfo
        {
            private string _name;

            /// <summary>
            /// Instantiates a CPU in the system, with the name and brand information.
            /// </summary>
            /// <param name="name">The raw CPU name.</param>
            public CPUInfo(string name)
            {
                string[] words = name.Split('_');
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].ToLower().Contains("intel(r)"))
                    {
                        CPUBrand = CPUBrand.Intel;
                        _name = words[i + 2];
                        return;
                    }
                    else if (words[i] == "AMD")
                    {
                        CPUBrand = CPUBrand.AMD;
                        _name = words[i + 1] + " " + words[i + 2] + " " + words[i + 3];
                        return;
                    }
                }
                _name = "Unknown CPU";
            }

            public string Name { get => _name; }
        }

        public class RAMInfo
        {
            private int _size;
            private int _speed;

            /// <summary>
            /// Instantiates RAM in the system, with a relevant description.
            /// </summary>
            /// <param name="desc">The relevant description.</param>
            public RAMInfo()
            {
                DMChecker.GetRAMDescription(ref _size, ref _speed);
            }

            public int Size { get => _size; set => _size = value; }
            public int Speed { get => _speed; set => _speed = value; }
        }
    }
}

