using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Parse.Regexp
{
    class PatternsContainer
    {
        public PatternsContainer(Uri uri, List<string> patterns)
        {
            Uri = uri;
            Validation = new ByTagsValidator(patterns);
        }

        public readonly Uri Uri;
        public readonly ByTagsValidator Validation;
    }
}
