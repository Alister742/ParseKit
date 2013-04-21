using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.DOMElements.Interfaces
{
    interface INodeList
    {
        Node item(int index);
        int length { get; }
    }
}
