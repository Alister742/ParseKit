using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.Validation
{
    public interface IPageValidator
    {
        bool Validate(string originalPage, string validatingPage);
    }
}
