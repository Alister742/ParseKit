using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Parse.Regexp
{
    class RegexParseAction : IParseAction
    {
        #region Члены IParseAction

        public object ParseField(object target, params object[] args)
        {
            if (args == null || args)
            {
                throw new ArgumentException("'param' value can't be null");
            }

            var pattern = args[0] as string;
            if (pattern == null)
            {
                throw new ArgumentException("'param' can't be cenverted to pattern string");
            }

            Regex rx = new Regex(pattern);

            return rx.;
        }

        #endregion
    }
}
