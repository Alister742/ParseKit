using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface IHTMLCollection
    {
        long length { get; }

        Element item(int index);

        object namedItem(string name);
    }
}
