using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Lists;
using ParseKit.DOMSupport.DOMElements.Nodes.Interfaces;

namespace Parse.DOM.DOMElements
{
    class MutationRecord : IMutationRecord
    {
        public string type { get; private set; }
        public Node target { get; private set; }
        public NodeList addedNodes { get; private set; }
        public NodeList removedNodes { get; private set; }
        public Node previousSibling { get; private set; }
        public Node nextSibling { get; private set; }
        public string attributeName { get; private set; }
        public string attributeNamespace { get; private set; }
        public string oldValue { get; private set; }
    };
}
