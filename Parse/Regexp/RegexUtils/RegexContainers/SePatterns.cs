using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Parse.Regexp
{
    public class SePatterns
    {
        public SePatterns(string banPattern, string validPattern)
        {
            BanRx = new Regex(banPattern);
            ValidRx = new Regex(validPattern);
        }

        public readonly Regex BanRx;
        public readonly Regex ValidRx;
    }
}
