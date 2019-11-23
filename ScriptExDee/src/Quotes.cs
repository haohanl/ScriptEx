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
            "Now 100% vegan-friendly",
            "Gah! Don't look at me that way",
            "Magware? Never heard of it",
            "Now accepting zip|money",
            "You didn't bend any pins. Bravo",
            "Now *supports* Jacob64",
            "Time for a 15-minute break -Noone",
            "She'll be right mate -Systems",
            "Forecast is 25 with a chance of BBs",
            "You should get more sleep you know",
            "Stop breakin' my balls -Robin",
            "You're done? But done is over there",
            "I'm going upstairs -Nathan",
            "RIP Fish'n'chip Fridays (2019-2019)",
            "Join the HiKoki appreciation club",
            
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
