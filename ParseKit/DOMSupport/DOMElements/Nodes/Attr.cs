using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMSupport.DOMElements.Nodes.Interfaces;

namespace ParseKit.DOMElements._Classes.Nodes
{
    class Attr : IAttr
    {
        public string localName { get; private set; }
        public string value { get; set; }

        public string name { get; private set; }
        public string? namespaceURI { get; private set; }
        public string? prefix { get; private set; }
    };
}
