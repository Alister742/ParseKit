using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse
{
    interface IParseAction
    {
        object ParseField(object target, params object[] args);
    }
}
