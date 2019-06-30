using System;

namespace battlepong_game.Utilities
{
    public static class Randomizer
    {
        public static double Generate(double min, double max)
        {
            return new Random().NextDouble() * (max - min) + min;
        }

        public static double Gaussian(double mean = 0, double stdDev = 1)
        {
            Random r = new Random();

            var u1 = r.NextDouble();
            var u2 = r.NextDouble();

            var randomStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            return mean + stdDev * randomStdNormal;
        }
    }
}
