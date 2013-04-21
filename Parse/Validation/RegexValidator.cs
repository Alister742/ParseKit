using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ParseKit;

namespace Parse.Validation
{
    class ByRegexValidator : IPageValidator
    {
        Regex _validationRx;

        public ByRegexValidator(Regex rx)
        {
            _validationRx = rx;
        }

        public bool Validate(string page)
        {
            return _validationRx.IsMatch(page);
        }
    }
}
