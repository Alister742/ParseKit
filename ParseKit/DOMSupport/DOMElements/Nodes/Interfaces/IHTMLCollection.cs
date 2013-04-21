using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;

namespace ParseKit.DOMSupport.DOMElements.Nodes.Interfaces
{
    interface IHTMLCollection
    {
        long length { get; }

        Element item(int index);

        object namedItem(string name);
    }
}
