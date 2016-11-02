using System;

namespace Edubase.Common
{
    /// <summary>
    /// Generates random numbers avoiding seed clash
    /// </summary>
    public static class RandomNumber
    {
        private static Random _global = new Random();

        [ThreadStatic]
        private static Random _local;

        public static int Next()
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next();
        }

        public static int Next(int min, int max)
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next(min, max);
        }
    }
}
