using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace labexam.ut
{
    internal static class EaseFunction
    {
        /// <summary>
        /// Linear
        /// </summary>
        /// <returns>
        /// Takes interpolation factor, does nothing with it, and returns it
        /// </returns>
        public static float Linear(float t)
            => t;


        /// <summary>
        /// Quadratic ease-in function
        /// </summary>
        public static float QuadIn(float t)
            => t * t;

        /// <summary>
        /// Quadratic ease-out function
        /// </summary>
        public static float QuadOut(float t)
            => 1 - (1 - t) * (1 - t);

        /// <summary>
        /// Quadratic ease-in-out function
        /// </summary>
        public static float QuadInOut(float t)
            => t < .5f
            ? 2 * t * t
            : 1 - float.Pow(-2 * t + 2, 2) / 2;


        /// <summary>
        /// Exponential ease-in function
        /// </summary>
        public static float ExpoIn(float t)
            => t == 0
            ? 0
            : float.Pow(2, 10 * (t - 1));

        /// <summary>
        /// Exponential ease-out function
        /// </summary>
        public static float ExpoOut(float t)
            => t == 1
            ? 1
            : 1 - float.Pow(2, -10 * t);

        /// <summary>
        /// Exponential ease-in-out function
        /// </summary>
        public static float ExpoInOut(float t)
        {
            if (t == 0) return 0;
            if (t == 1) return 1;

            return t < .5f
            ? .5f * float.Pow(2, 20 * t - 10)
            : 1 - .5f * float.Pow(2, -20 * t * 10);
        }
    }
}
