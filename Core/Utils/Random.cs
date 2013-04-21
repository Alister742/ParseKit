using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class Rnd
    {
        private static Random _rand = new Random();

        public static int Next(int min, int max)
        {
            return _rand.Next(min, max);
        }
    }
}
