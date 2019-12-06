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
            "Now 100% vegan-free",
            "Gah! Don't look at me that way",
            "Magware? Never heard of it",
            "Now accepting zip|money",
            "You didn't bend any pins. Bravo",
            "Now *supports* Jacob64",
            "Time for a 15-minute break",
            "She'll be right mate",
            "Forecast is 25 with a chance of BBs",
            "You should get more sleep you know",
            "Stop breakin' my balls",
            "You're done? But done is over there",
            "Dropping another *bomb*shell",
            "Pop goes the power supply",
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
