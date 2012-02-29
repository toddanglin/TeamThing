using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Data.Util
{
    public static class RandomPhrase
    {
        private static Random random = new Random();

        public static string GetRandomGreeting()
        {
            List<string> greetings = new List<string>()
            {
                "Hello",
                "G'day",
                "Hey, ",
                "Howdy",
                "Greetings",
                "01101000 01101001",
                "Hi",
                "Aloha"
            };

            if (DateTime.Now.TimeOfDay >= new TimeSpan(6, 0, 0) && DateTime.Now.TimeOfDay < new TimeSpan(12, 0, 0))
                greetings.Add("Good morning");
            if (DateTime.Now.TimeOfDay >= new TimeSpan(12, 0, 0) && DateTime.Now.TimeOfDay < new TimeSpan(17, 0, 0))
                greetings.Add("Good afternoon");
            if (DateTime.Now.TimeOfDay >= new TimeSpan(17, 0, 0) && DateTime.Now.TimeOfDay <= new TimeSpan(23, 59, 59))
                greetings.Add("Good evening");
            if (DateTime.Now.TimeOfDay >= new TimeSpan(0, 0, 0) && DateTime.Now.TimeOfDay < new TimeSpan(6, 0, 0))
                greetings.Add("Good early morning");

            //Pick a random phrase to return
            return GetRandomString(greetings);
        }

        public static string GetRandomClosing()
        {
            List<string> closings = new List<string>()
            {
                "Have a good day!",
                "Your simple helper,",
                "Here to help,",
                "Working for you,",
                "Back to work!",
                "Til next time,"
            };

            //Pick a random phrase to return
            return GetRandomString(closings);
        }

        private static string GetRandomString(IList<string> strings)
        {
            //Pick a random phrase to return
            int index = 0;
            lock (random)
            {
                index = random.Next(0, strings.Count() - 1);
            }

            return strings[index];
        }
    }
}
