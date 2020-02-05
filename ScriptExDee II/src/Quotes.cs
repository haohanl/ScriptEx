using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExDee_II
{
    static class Quotes
    {
        public static string[] Entries = new string[] { 
            "Now 100% vegan-free",
            "Gah! Don't look at me that way",
            "Magware? Never heard of it",
            "Now accepting zip|money",
            "You didn't bend any pins. Bravo",
            "Now \"supports\" Jacob64",
            "Time for a 15-minute break",
            "She'll be right mate",
            "You should get more sleep you know",
            "Stop breakin' my balls",
            "You're done? But done is over there",
            "Dropping another bombshell",
            "Pop goes the power supply",
            "Hey, you feelin' rich?",
            "Sick smoke machine bro",
            "Would be a shame if the RAM is faulty",
            "Hope those fans are spinning",
            "Electric boogaloo",
            "What's that noise?",
            "QC fail, not enough RGB"
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
