using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;

namespace ParseKit.DOMElements._Classes.Traversal
{
    class NodeIterator
    {
        public Node root { get; private set; }
        public Node? referenceNode { get; private set; }
        public bool pointerBeforeReferenceNode { get; private set; }
        public long whatToShow { get; private set; }
        public NodeFilter? filter { get; private set; }

        public Node? nextNode();
        public Node? previousNode();

        public void detach();
    };
}
