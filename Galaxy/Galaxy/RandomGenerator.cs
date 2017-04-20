using System;

namespace Galaxy
{
    static class RandomGenerator
    {
        public static Random Generator = new Random(DateTime.Now.Millisecond);
    }
}
