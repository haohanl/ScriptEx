using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExDee
{
    static class Quotes
    {
        public static string[] Entries = new string[] { 
            "It's not a virus, I swear!",
            "RGB stands for Real Gnarly BBs",
            "Guaranteed to be 100% fat-free",
            "Gah! Don't look at me that way",
            "Magware? Who's that?",
            "H gon' give it to ya",
            "Now accepting zip|money",
            "You didn't bend any pins. Bravo!",
            "Now *supports* Jacob64",
            "Time for a 15-minute break"
        };

        /// <summary>
        /// Return a random quote.
        /// </summary>
        /// <returns></returns>
        public static string GetQuote()
        {
            Random rnd = new Random();
            int rndInt = rnd.Next(0, Entries.Length);
            return Entries[rndInt];
        }
    }
}
