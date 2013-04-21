using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface IText
    {
        Text splitText(int offset);
        string wholeText { get; }
    }
}
