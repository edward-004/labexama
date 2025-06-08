using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace labexam.ut
{
    internal static class Rand
    {
        static readonly Random r = new();

        /// <summary>
        /// Generates a random integer within a specified range
        /// </summary>
        public static int Range(int min, int max) => r.Next(min, max + 1);

        public static float Single() => r.NextSingle();

        /// <summary>
        /// Between true or false
        /// </summary>
        public static bool Switch() => r.Next(2) == 0;

        public static float RangeF(float min, float max) => min + (r.NextSingle() * (max - min));
    }
}
