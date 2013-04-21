using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface INodeIterator
    {
        Node root { get; }
        Node referenceNode { get; }
        bool pointerBeforeReferenceNode { get; }
        long whatToShow { get; }
        NodeFilter filter { get; }

        Node nextNode();
        Node previousNode();

        void detach();
    }
}
