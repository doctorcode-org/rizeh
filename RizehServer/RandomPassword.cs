namespace Parsnet
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    public class RandomPassword
    {
        public static string Create()
        {
            return Create(8, false);
        }

        public static string Create(int len, bool onlyNumber = false)
        {
            string element = onlyNumber ? "0123456789" : "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            Random random = new Random();
            return new string((from s in Enumerable.Repeat<string>(element, len) select s[random.Next(s.Length)]).ToArray<char>());
        }
    }
}

