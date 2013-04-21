using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMSupport.DOMElements.Nodes.Interfaces
{
    interface IAttr
    {
        string localName { get; }
        string value { get; set; }

        string name { get; }
        string? namespaceURI { get; }
        string? prefix { get; }
    }
}
