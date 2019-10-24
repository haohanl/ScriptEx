using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlinkStickDotNet;

namespace ScriptEx
{
    static class BlinkStickHandler
    {
        public static bool stopTrigger = false;


        static BlinkStick device;

        public static void Initialise()
        {
            device = BlinkStick.FindFirst();
            if (device != null && device.OpenDevice())
            {
                Terminal.WriteLine("@", "BlinkStick initialised.");



                Reset();


                Success(24);

                QuadSiren(24);
                Siren(24);

                Alert(24);

                Idle(24);
                //while (true)
                //{
                //    Siren(800, "white", "blue");
                //}

                //try
                //{
                //    for (int i = 0; i < 8; i++)
                //    {
                //        Thread.Sleep(200);
                //        Thread thr = new Thread(() => 
                //            device.Morph(0, (byte)i, 
                //                (byte)(i * 30), 
                //                (byte)(255 - i * 30), 
                //                (byte)(255 - i * 30), 
                //                100, 10));
                //        thr.Start();

                //    }
                //}
                //catch (Exception)
                //{
                //    Terminal.WriteLine("~", "Slight BlinkStick Error.");
                //}
                Reset();
            }
        }

        public static void Start()
        {
            Thread thr = new Thread(Initialise);
            thr.Start();
        }


        public static void Siren()
        {
            Siren(8, 4);
        }

        public static void Siren(int cycles)
        {
            Siren(cycles, 4);
        }

        static void Siren(int cycles, int delay, string prim_colour = "red", string sec_colour = "blue")
        {
            // Do Not Set delay to be less than 4. It will send too many signals and crash.
            int count = 0;
            int index = 0;

            while (count < cycles)
            {
                index = count % 8;

                // Pattern & Colours
                Thread primary = new Thread(() => 
                    device.Morph(0, (byte)index, prim_colour, 80, delay));
                Thread secondary = new Thread(() => 
                    device.Morph(0, (byte)((index + 4) % 8), sec_colour, 80, delay));

                primary.Start();
                secondary.Start();

                primary.Join();
                secondary.Join();

                count++;
            }
        }

        public static void QuadSiren()
        {
            QuadSiren(8, 10);
        }

        public static void QuadSiren(int cycles)
        {
            QuadSiren(cycles, 10);
        }
        static void QuadSiren(int cycles, int delay, string colour_one = "red", string colour_two = "blue", string colour_three = "red", string colour_four = "blue")
        {
            // Do Not Set delay to be less than 4. It will send too many signals and crash.
            int count = 0;
            int index = 0;

            while (count < cycles)
            {
                index = count % 8;

                // Pattern & Colours
                Thread one = new Thread(() =>
                    device.Morph(0, (byte)index, colour_one, 80, delay));
                Thread two = new Thread(() =>
                    device.Morph(0, (byte)((index + 2) % 8), colour_two, 80, delay));
                Thread three = new Thread(() =>
                    device.Morph(0, (byte)((index + 4) % 8), colour_three, 80, delay));
                Thread four = new Thread(() =>
                    device.Morph(0, (byte)((index + 6) % 8), colour_four, 80, delay));

                one.Start();
                two.Start();
                three.Start();
                four.Start();

                one.Join();
                two.Join();
                three.Join();
                four.Join();

                count++;
            }
        }


        public static void Alert()
        {
            Alert(8, 8);
        }
        public static void Alert(int cycles)
        {
            Alert(cycles, 8);
        }
        static void Alert(int cycles, int delay, string prim_colour = "red", string sec_colour = "yellow")
        {
            Siren(cycles, delay, prim_colour, sec_colour);
        }


        public static void Idle()
        {
            Idle(8, 15);
        }
        public static void Idle(int cycles)
        {
            Idle(cycles, 15);
        }
        static void Idle(int cycles, int delay, string prim_colour = "white", string sec_colour = "blue")
        {
            Siren(cycles, delay, prim_colour, sec_colour);
        }

        public static void Success()
        {
            Success(8, 15);
        }
        public static void Success(int cycles)
        {
            Success(cycles, 15);
        }
        static void Success(int cycles, int delay, string colour_one = "green", string colour_two = "green", string colour_three = "green", string colour_four = "blue")
        {
            QuadSiren(cycles, delay, colour_one, colour_two, colour_three, colour_four);
        }


        static void Reset()
        {
            for (byte i = 0; i < 8; i++)
            {
                device.SetColor(0, i, "Black");
            }
        }

        static void Break()
        {
            stopTrigger = true;
        }
        static void Fix()
        {
            stopTrigger = false;
        }

    }
}
