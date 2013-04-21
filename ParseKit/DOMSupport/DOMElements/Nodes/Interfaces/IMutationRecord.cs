using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;
using ParseKit.DOMElements._Classes.Lists;

namespace ParseKit.DOMSupport.DOMElements.Nodes.Interfaces
{
    interface IMutationRecord
    {
        string type { get; }
        Node target { get; }
        NodeList addedNodes { get; }
        NodeList removedNodes { get; }
        Node? previousSibling { get; }
        Node? nextSibling { get; }
        string? attributeName { get; }
        string? attributeNamespace { get; }
        string? oldValue { get; }
    }
}
