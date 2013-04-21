using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Parse.Regexp
{
    class AnonymousRegexes
    {
        public AnonymousRegexes(string uri, string hightAnonymPatern, string anonymPatern)
        {
            Uri = new Uri(uri);
            HightAnonymous = new Regex(hightAnonymPatern);
            Anonymous = new Regex(anonymPatern);
        }

        public readonly Uri Uri;
        public readonly Regex HightAnonymous;
        public readonly Regex Anonymous;
    }
}
