using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface IMutationRecord
    {
        string type { get; }
        Node target { get; }
        NodeList addedNodes { get; }
        NodeList removedNodes { get; }
        Node previousSibling { get; }
        Node nextSibling { get; }
        string attributeName { get; }
        string attributeNamespace { get; }
        string oldValue { get; }
    }
}
